using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using BookingRoomInfo = AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo;

namespace AvenSuitesApi.Application.Services.Implementations.Booking;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingRoomRepository _bookingRoomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IRatePlanRepository _ratePlanRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        IBookingRoomRepository bookingRoomRepository,
        IHotelRepository hotelRepository,
        IGuestRepository guestRepository,
        IRoomRepository roomRepository,
        IRoomTypeRepository roomTypeRepository,
        IRatePlanRepository ratePlanRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _bookingRoomRepository = bookingRoomRepository;
        _hotelRepository = hotelRepository;
        _guestRepository = guestRepository;
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
        _ratePlanRepository = ratePlanRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<BookingResponse?> CreateBookingAsync(BookingCreateRequest request)
    {
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
        if (hotel == null)
            return null;

        var mainGuest = await _guestRepository.GetByUserId(request.MainGuestId);
        if (mainGuest == null)
            return null;

        foreach (var bookingRoom in request.BookingRooms)
        {
            var room = await _roomRepository.GetByIdAsync(bookingRoom.RoomId);
            if (room == null || room.HotelId != request.HotelId)
                return null;
            
            var isAvailable = await IsRoomAvailableForDatesAsync(
                bookingRoom.RoomId, 
                request.CheckInDate, 
                request.CheckOutDate);
            
            if (!isAvailable)
                return null;
        }

        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            HotelId = request.HotelId,
            Code = request.Code,
            Status = "PENDING",
            Source = request.Source,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Adults = request.Adults,
            Children = request.Children,
            Currency = request.Currency,
            MainGuestId = mainGuest.Id,
            ChannelRef = request.ChannelRef,
            Notes = request.Notes,
            TotalAmount = 0, // Será calculado abaixo baseado na ocupação
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Calcular preços dos quartos baseado na ocupação antes de criar o booking
        var calculatedPrices = new Dictionary<Guid, decimal>();
        var totalOccupancy = (short)(request.Adults + request.Children);
        var nights = (request.CheckOutDate - request.CheckInDate).Days;

        foreach (var bookingRoomRequest in request.BookingRooms)
        {
            // Buscar RoomType com preços de ocupação
            var roomType = await _roomTypeRepository.GetByIdWithOccupancyPricesAsync(bookingRoomRequest.RoomTypeId);
            if (roomType == null)
                return null; // RoomType não encontrado

            // Calcular preço por noite baseado na ocupação
            var pricePerNight = GetPriceForOccupancy(roomType, totalOccupancy);
            
            // Calcular preço total para o quarto (preço por noite × número de noites)
            var totalPrice = CalculateTotalPrice(pricePerNight, request.CheckInDate, request.CheckOutDate);
            
            calculatedPrices[bookingRoomRequest.RoomId] = totalPrice;
        }

        // Atualizar TotalAmount do booking com os preços calculados
        booking.TotalAmount = calculatedPrices.Values.Sum();

        var createdBooking = await _bookingRepository.AddAsync(booking);
        
        foreach (var bookingRoomRequest in request.BookingRooms)
        {
            // Usar o preço calculado ao invés do preço do request
            var calculatedPrice = calculatedPrices[bookingRoomRequest.RoomId];
            
            var bookingRoom = new AvenSuitesApi.Domain.Entities.BookingRoom
            {
                Id = Guid.NewGuid(),
                BookingId = createdBooking.Id,
                RoomId = bookingRoomRequest.RoomId,
                RoomTypeId = bookingRoomRequest.RoomTypeId,
                RatePlanId = bookingRoomRequest.RatePlanId,
                PriceTotal = calculatedPrice, // Usar preço calculado
                Notes = bookingRoomRequest.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _bookingRoomRepository.AddAsync(bookingRoom);

            createdBooking.BookingRooms.Add(bookingRoom);
        }
        
        var response = MapToResponse(createdBooking);
        
        // Enviar emails para hóspede e hotel em background
        _ = Task.Run(async () => 
        {
            await SendBookingConfirmationEmailAsync(createdBooking);
            await SendHotelBookingNotificationEmailAsync(createdBooking);
        });
        
        return response;
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
        if (booking == null)
            return null;

        return MapToResponse(booking);
    }

    public async Task<BookingResponse?> GetBookingByCodeAsync(Guid hotelId, string code)
    {
        var booking = await _bookingRepository.GetByCodeAsync(hotelId, code);
        if (booking == null)
            return null;

        return MapToResponse(booking);
    }

    public async Task<IEnumerable<BookingResponse>> GetBookingsByHotelAsync(Guid hotelId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var bookings = startDate.HasValue && endDate.HasValue
            ? await _bookingRepository.GetByHotelIdAndDatesAsync(hotelId, startDate.Value, endDate.Value)
            : await _bookingRepository.GetByHotelIdAsync(hotelId);

        return bookings.Select(MapToResponse);
    }

    public async Task<IEnumerable<BookingResponse>> GetBookingsByGuestAsync(Guid guestId)
    {
        var bookings = await _bookingRepository.GetByGuestIdAsync(guestId);
        return bookings.Select(MapToResponse);
    }

    public async Task<BookingResponse?> UpdateBookingAsync(Guid id, BookingUpdateRequest request)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return null;

        string? oldStatus = booking.Status;

        if (!string.IsNullOrEmpty(request.Status) && request.Status != oldStatus)
        {
            // Registrar mudança de status
            await RegisterStatusChangeAsync(booking.Id, oldStatus, request.Status, null);
            booking.Status = request.Status;
        }

        if (request.CheckInDate.HasValue)
            booking.CheckInDate = request.CheckInDate.Value;

        if (request.CheckOutDate.HasValue)
            booking.CheckOutDate = request.CheckOutDate.Value;

        if (request.Adults.HasValue)
            booking.Adults = request.Adults.Value;

        if (request.Children.HasValue)
            booking.Children = request.Children.Value;

        if (!string.IsNullOrEmpty(request.Notes))
            booking.Notes = request.Notes;

        booking.UpdatedAt = DateTime.UtcNow;

        var updatedBooking = await _bookingRepository.UpdateAsync(booking);
        return MapToResponse(updatedBooking);
    }
    
    private async Task RegisterStatusChangeAsync(Guid bookingId, string? oldStatus, string newStatus, string? notes)
    {
        // Nota: Você precisará injetar ApplicationDbContext para salvar o histórico
        var history = new BookingStatusHistory
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow,
            Notes = notes
        };
    }

    public async Task<bool> CancelBookingAsync(Guid id, string? reason = null)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
        if (booking == null)
            return false;

        var oldStatus = booking.Status;
        booking.Status = "CANCELLED";
        booking.Notes = string.IsNullOrEmpty(reason) ? booking.Notes : reason;
        booking.UpdatedAt = DateTime.UtcNow;

        await RegisterStatusChangeAsync(booking.Id, oldStatus, "CANCELLED", reason);
        await _bookingRepository.UpdateAsync(booking);
        
        // Enviar emails para hóspede e hotel em background
        _ = Task.Run(async () => 
        {
            await SendBookingCancellationEmailAsync(booking, reason);
            await SendHotelBookingCancellationEmailAsync(booking, reason);
        });
        
        return true;
    }

    public async Task<bool> ConfirmBookingAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
        if (booking == null)
            return false;

        var oldStatus = booking.Status;
        booking.Status = "CONFIRMED";
        booking.UpdatedAt = DateTime.UtcNow;

        await RegisterStatusChangeAsync(booking.Id, oldStatus, "CONFIRMED", "Reserva confirmada via API");
        await _bookingRepository.UpdateAsync(booking);
        
        // Enviar emails para hóspede e hotel em background
        _ = Task.Run(async () => 
        {
            await SendBookingConfirmedEmailAsync(booking);
            await SendHotelBookingConfirmedEmailAsync(booking);
        });
        
        return true;
    }

    public async Task<bool> CheckInAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CHECKED_IN";
        booking.UpdatedAt = DateTime.UtcNow;

        foreach (var bookingRoom in booking.BookingRooms)
        {
            var room = await _roomRepository.GetByIdAsync(bookingRoom.RoomId);
            if (room != null)
            {
                room.Status = "OCCUPIED";
                room.UpdatedAt = DateTime.UtcNow;
                await _roomRepository.UpdateAsync(room);
            }
        }

        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> CheckOutAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CHECKED_OUT";
        booking.UpdatedAt = DateTime.UtcNow;

        foreach (var bookingRoom in booking.BookingRooms)
        {
            var room = await _roomRepository.GetByIdAsync(bookingRoom.RoomId);
            if (room != null)
            {
                room.Status = "CLEANING";
                room.UpdatedAt = DateTime.UtcNow;
                await _roomRepository.UpdateAsync(room);
            }
        }

        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    private static BookingResponse MapToResponse(AvenSuitesApi.Domain.Entities.Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            Code = booking.Code,
            Status = booking.Status,
            Source = booking.Source,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            Adults = booking.Adults,
            Children = booking.Children,
            Currency = booking.Currency,
            TotalAmount = booking.TotalAmount,
            MainGuestId = booking.MainGuestId,
            ChannelRef = booking.ChannelRef,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            MainGuest = booking.MainGuest != null ? new GuestSummaryResponse
            {
                Id = booking.MainGuest.Id,
                FullName = booking.MainGuest.GuestPii?.FullName,
                Email = booking.MainGuest.GuestPii?.Email,
                Phone = booking.MainGuest.GuestPii?.PhoneE164
            } : null,
            BookingRooms = booking.BookingRooms.Select(br => new BookingRoomResponse
            {
                Id = br.Id,
                RoomId = br.RoomId,
                RoomNumber = br.Room.RoomNumber,
                RoomTypeName = br.RoomType?.Name ?? "",
                PriceTotal = br.PriceTotal,
                Notes = br.Notes ?? ""
            }).ToList(),
            Payments = booking.Payments.Select(p => new BookingPaymentResponse
            {
                Id = p.Id,
                Method = p.Method,
                Status = p.Status,
                Amount = p.Amount,
                Currency = p.Currency,
                PaidAt = p.PaidAt
            }).ToList()
        };
    }
    
    private async Task<bool> IsRoomAvailableForDatesAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null || room.Status != "ACTIVE")
            return false;

        var activeBookings = await _bookingRepository.GetByHotelIdAsync(room.HotelId);
        var hasConflict = activeBookings.Any(b => 
            b.Status != "CANCELLED"
            && b.BookingRooms.Any(br => br.RoomId == roomId)
            && b.CheckInDate < checkOut 
            && b.CheckOutDate > checkIn);

        return !hasConflict;
    }
    
    private static decimal CalculateTotalFromRequest(BookingCreateRequest request)
    {
        // Este método não é mais usado, mas mantido para compatibilidade
        // O cálculo agora é feito no CreateBookingAsync usando ocupação
        var days = (request.CheckOutDate - request.CheckInDate).Days;
        return request.BookingRooms.Sum(br => br.PriceTotal) * days;
    }

    /// <summary>
    /// Obtém o preço por noite baseado na ocupação.
    /// Busca primeiro um preço específico para a ocupação no RoomTypeOccupancyPrice.
    /// Se não encontrar, usa o BasePrice do RoomType.
    /// </summary>
    private static decimal GetPriceForOccupancy(RoomType roomType, short occupancy)
    {
        // Buscar preço específico para a ocupação
        var occupancyPrice = roomType.OccupancyPrices?
            .FirstOrDefault(op => op.Occupancy == occupancy);

        // Se encontrar preço específico, usar ele; caso contrário, usar BasePrice
        return occupancyPrice?.PricePerNight ?? roomType.BasePrice;
    }

    /// <summary>
    /// Calcula o preço total baseado no preço por noite e no período de estadia.
    /// </summary>
    private static decimal CalculateTotalPrice(decimal pricePerNight, DateTime checkIn, DateTime checkOut)
    {
        var nights = (checkOut - checkIn).Days;
        return pricePerNight * nights;
    }
    
    private async Task SendBookingConfirmationEmailAsync(Domain.Entities.Booking booking)
    {
        try
        {
            var bookingWithDetails = await _bookingRepository.GetByIdWithDetailsAsync(booking.Id);
            if (bookingWithDetails == null)
                return;

            var hotel = await _hotelRepository.GetByIdAsync(bookingWithDetails.HotelId);
            if (hotel == null)
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(bookingWithDetails.MainGuestId);
            if (guest?.GuestPii == null || string.IsNullOrWhiteSpace(guest.GuestPii.Email))
                return;

            var nights = (bookingWithDetails.CheckOutDate - bookingWithDetails.CheckInDate).Days;
            var rooms = bookingWithDetails.BookingRooms
                .Where(br => br.Room != null && br.RoomType != null)
                .GroupBy(br => br.RoomId)
                .Select(g => g.First())
                .Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room!.RoomNumber,
                    RoomTypeName = br.RoomType!.Name ?? "",
                    PriceTotal = br.PriceTotal
                })
                .ToList();

            if (rooms.Count == 0)
                return;

            var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                : null;

            var emailBody = _emailTemplateService.GenerateBookingConfirmationEmail(
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                hotelName: hotel.Name,
                bookingCode: bookingWithDetails.Code,
                checkInDate: bookingWithDetails.CheckInDate,
                checkOutDate: bookingWithDetails.CheckOutDate,
                nights: nights,
                totalAmount: bookingWithDetails.TotalAmount,
                currency: bookingWithDetails.Currency,
                rooms: rooms,
                hotelAddress: hotelAddress,
                hotelPhone: hotel.PhoneE164);

            await _emailService.SendEmailAsync(
                to: guest.GuestPii.Email,
                subject: $"Confirmação de Reserva - {hotel.Name}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de confirmação de reserva enviado para {Email}, Booking: {BookingCode}",
                guest.GuestPii.Email, bookingWithDetails.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de confirmação de reserva para Booking: {BookingId}",
                booking.Id);
        }
    }
    
    private async Task SendBookingConfirmedEmailAsync(Domain.Entities.Booking booking)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
            if (hotel == null)
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(booking.MainGuestId);
            if (guest?.GuestPii == null || string.IsNullOrWhiteSpace(guest.GuestPii.Email))
                return;

            var nights = (booking.CheckOutDate - booking.CheckInDate).Days;
            var rooms = booking.BookingRooms
                .Where(br => br.Room != null && br.RoomType != null)
                .GroupBy(br => br.RoomId)
                .Select(g => g.First())
                .Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room!.RoomNumber,
                    RoomTypeName = br.RoomType!.Name ?? "",
                    PriceTotal = br.PriceTotal
                })
                .ToList();

            if (rooms.Count == 0)
                return;

            var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                : null;

            var emailBody = _emailTemplateService.GenerateBookingConfirmedEmail(
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                hotelName: hotel.Name,
                bookingCode: booking.Code,
                checkInDate: booking.CheckInDate,
                checkOutDate: booking.CheckOutDate,
                nights: nights,
                rooms: rooms,
                hotelAddress: hotelAddress,
                hotelPhone: hotel.PhoneE164);

            await _emailService.SendEmailAsync(
                to: guest.GuestPii.Email,
                subject: $"Reserva Confirmada - {hotel.Name}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de reserva confirmada enviado para {Email}, Booking: {BookingCode}",
                guest.GuestPii.Email, booking.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de reserva confirmada para Booking: {BookingId}",
                booking.Id);
        }
    }
    
    private async Task SendBookingCancellationEmailAsync(Domain.Entities.Booking booking, string? reason = null)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
            if (hotel == null)
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(booking.MainGuestId);
            if (guest?.GuestPii == null || string.IsNullOrWhiteSpace(guest.GuestPii.Email))
                return;

            var emailBody = _emailTemplateService.GenerateBookingCancellationEmail(
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                hotelName: hotel.Name,
                bookingCode: booking.Code,
                checkInDate: booking.CheckInDate,
                checkOutDate: booking.CheckOutDate,
                totalAmount: booking.TotalAmount,
                currency: booking.Currency,
                reason: reason);

            await _emailService.SendEmailAsync(
                to: guest.GuestPii.Email,
                subject: $"Cancelamento de Reserva - {hotel.Name}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de cancelamento de reserva enviado para {Email}, Booking: {BookingCode}",
                guest.GuestPii.Email, booking.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de cancelamento de reserva para Booking: {BookingId}",
                booking.Id);
        }
    }

    /// <summary>
    /// Envia e-mail de notificação de nova reserva para o hotel
    /// </summary>
    private async Task SendHotelBookingNotificationEmailAsync(Domain.Entities.Booking booking)
    {
        try
        {
            var bookingWithDetails = await _bookingRepository.GetByIdWithDetailsAsync(booking.Id);
            if (bookingWithDetails == null)
                return;

            var hotel = await _hotelRepository.GetByIdAsync(bookingWithDetails.HotelId);
            if (hotel == null || string.IsNullOrWhiteSpace(hotel.Email))
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(bookingWithDetails.MainGuestId);
            if (guest?.GuestPii == null)
                return;

            var nights = (bookingWithDetails.CheckOutDate - bookingWithDetails.CheckInDate).Days;
            var rooms = bookingWithDetails.BookingRooms
                .Where(br => br.Room != null && br.RoomType != null)
                .GroupBy(br => br.RoomId)
                .Select(g => g.First())
                .Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room!.RoomNumber,
                    RoomTypeName = br.RoomType!.Name ?? "",
                    PriceTotal = br.PriceTotal
                })
                .ToList();

            if (rooms.Count == 0)
                return;

            var emailBody = _emailTemplateService.GenerateHotelBookingNotificationEmail(
                hotelName: hotel.Name,
                bookingCode: bookingWithDetails.Code,
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                guestEmail: guest.GuestPii.Email ?? "",
                guestPhone: guest.GuestPii.PhoneE164,
                checkInDate: bookingWithDetails.CheckInDate,
                checkOutDate: bookingWithDetails.CheckOutDate,
                nights: nights,
                adults: bookingWithDetails.Adults,
                children: bookingWithDetails.Children,
                totalAmount: bookingWithDetails.TotalAmount,
                currency: bookingWithDetails.Currency,
                rooms: rooms,
                notes: bookingWithDetails.Notes,
                channelRef: bookingWithDetails.ChannelRef);

            await _emailService.SendEmailAsync(
                to: hotel.Email,
                subject: $"Nova Reserva Recebida - {bookingWithDetails.Code}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de notificação de nova reserva enviado para hotel {HotelEmail}, Booking: {BookingCode}",
                hotel.Email, bookingWithDetails.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de notificação de nova reserva para hotel, Booking: {BookingId}",
                booking.Id);
        }
    }

    /// <summary>
    /// Envia e-mail de notificação de confirmação de reserva para o hotel
    /// </summary>
    private async Task SendHotelBookingConfirmedEmailAsync(Domain.Entities.Booking booking)
    {
        try
        {
            var bookingWithDetails = await _bookingRepository.GetByIdWithDetailsAsync(booking.Id);
            if (bookingWithDetails == null)
                return;

            var hotel = await _hotelRepository.GetByIdAsync(bookingWithDetails.HotelId);
            if (hotel == null || string.IsNullOrWhiteSpace(hotel.Email))
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(bookingWithDetails.MainGuestId);
            if (guest?.GuestPii == null)
                return;

            var nights = (bookingWithDetails.CheckOutDate - bookingWithDetails.CheckInDate).Days;
            var rooms = bookingWithDetails.BookingRooms
                .Where(br => br.Room != null && br.RoomType != null)
                .GroupBy(br => br.RoomId)
                .Select(g => g.First())
                .Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room!.RoomNumber,
                    RoomTypeName = br.RoomType!.Name ?? "",
                    PriceTotal = br.PriceTotal
                })
                .ToList();

            if (rooms.Count == 0)
                return;

            var emailBody = _emailTemplateService.GenerateHotelBookingNotificationEmail(
                hotelName: hotel.Name,
                bookingCode: bookingWithDetails.Code,
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                guestEmail: guest.GuestPii.Email ?? "",
                guestPhone: guest.GuestPii.PhoneE164,
                checkInDate: bookingWithDetails.CheckInDate,
                checkOutDate: bookingWithDetails.CheckOutDate,
                nights: nights,
                adults: bookingWithDetails.Adults,
                children: bookingWithDetails.Children,
                totalAmount: bookingWithDetails.TotalAmount,
                currency: bookingWithDetails.Currency,
                rooms: rooms,
                notes: $"Reserva CONFIRMADA - {bookingWithDetails.Notes}",
                channelRef: bookingWithDetails.ChannelRef);

            await _emailService.SendEmailAsync(
                to: hotel.Email,
                subject: $"Reserva Confirmada - {bookingWithDetails.Code}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de notificação de reserva confirmada enviado para hotel {HotelEmail}, Booking: {BookingCode}",
                hotel.Email, bookingWithDetails.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de notificação de reserva confirmada para hotel, Booking: {BookingId}",
                booking.Id);
        }
    }

    /// <summary>
    /// Envia e-mail de notificação de cancelamento de reserva para o hotel
    /// </summary>
    private async Task SendHotelBookingCancellationEmailAsync(Domain.Entities.Booking booking, string? reason = null)
    {
        try
        {
            var bookingWithDetails = await _bookingRepository.GetByIdWithDetailsAsync(booking.Id);
            if (bookingWithDetails == null)
                return;

            var hotel = await _hotelRepository.GetByIdAsync(bookingWithDetails.HotelId);
            if (hotel == null || string.IsNullOrWhiteSpace(hotel.Email))
                return;

            var guest = await _guestRepository.GetByIdWithPiiAsync(bookingWithDetails.MainGuestId);
            if (guest?.GuestPii == null)
                return;

            var emailBody = _emailTemplateService.GenerateHotelBookingCancellationEmail(
                hotelName: hotel.Name,
                bookingCode: bookingWithDetails.Code,
                guestName: guest.GuestPii.FullName ?? "Hóspede",
                guestEmail: guest.GuestPii.Email ?? "",
                checkInDate: bookingWithDetails.CheckInDate,
                checkOutDate: bookingWithDetails.CheckOutDate,
                totalAmount: bookingWithDetails.TotalAmount,
                currency: bookingWithDetails.Currency,
                reason: reason);

            await _emailService.SendEmailAsync(
                to: hotel.Email,
                subject: $"Cancelamento de Reserva - {bookingWithDetails.Code}",
                body: emailBody,
                isHtml: true,
                cc: null,
                bcc: null);

            _logger.LogInformation("E-mail de notificação de cancelamento enviado para hotel {HotelEmail}, Booking: {BookingCode}",
                hotel.Email, bookingWithDetails.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de notificação de cancelamento para hotel, Booking: {BookingId}",
                booking.Id);
        }
    }
}

