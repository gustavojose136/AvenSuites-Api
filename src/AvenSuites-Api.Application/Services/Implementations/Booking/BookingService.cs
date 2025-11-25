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
        _ratePlanRepository = ratePlanRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<BookingResponse?> CreateBookingAsync(BookingCreateRequest request)
    {
        // Validar hotel
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
        if (hotel == null)
            return null;

        // Validar hóspede principal
        var mainGuest = await _guestRepository.GetByUserId(request.MainGuestId);
        if (mainGuest == null)
            return null;

        // Validar e verificar disponibilidade dos quartos
        foreach (var bookingRoom in request.BookingRooms)
        {
            var room = await _roomRepository.GetByIdAsync(bookingRoom.RoomId);
            if (room == null || room.HotelId != request.HotelId)
                return null; // Quarto não encontrado ou não pertence ao hotel
            
            // Verificar disponibilidade
            var isAvailable = await IsRoomAvailableForDatesAsync(
                bookingRoom.RoomId, 
                request.CheckInDate, 
                request.CheckOutDate);
            
            if (!isAvailable)
                return null; // Quarto não disponível
        }

        // Criar booking
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
            TotalAmount = CalculateTotalFromRequest(request),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBooking = await _bookingRepository.AddAsync(booking);
        
        // Criar booking rooms
        foreach (var bookingRoomRequest in request.BookingRooms)
        {
            var bookingRoom = new AvenSuitesApi.Domain.Entities.BookingRoom
            {
                Id = Guid.NewGuid(),
                BookingId = createdBooking.Id,
                RoomId = bookingRoomRequest.RoomId,
                RoomTypeId = bookingRoomRequest.RoomTypeId,
                RatePlanId = bookingRoomRequest.RatePlanId,
                PriceTotal = bookingRoomRequest.PriceTotal,
                Notes = bookingRoomRequest.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Adicionar usando o repositório (seguindo SOLID)
            await _bookingRoomRepository.AddAsync(bookingRoom);

            createdBooking.BookingRooms.Add(bookingRoom);
        }
        
        // Buscar dados completos para o e-mail
        var bookingWithDetails = await _bookingRepository.GetByIdWithDetailsAsync(createdBooking.Id);
        if (bookingWithDetails != null)
        {
            // Enviar e-mail de confirmação de reserva
            try
            {
                var guestPii = bookingWithDetails.MainGuest?.GuestPii;
                var guestEmail = guestPii?.Email;
                var guestName = guestPii?.FullName ?? "Hóspede";
                
                if (!string.IsNullOrWhiteSpace(guestEmail))
                {
                    var nights = (bookingWithDetails.CheckOutDate - bookingWithDetails.CheckInDate).Days;
                    var rooms = bookingWithDetails.BookingRooms.Select(br => new BookingRoomInfo
                    {
                        RoomNumber = br.Room?.RoomNumber ?? "",
                        RoomTypeName = br.RoomType?.Name ?? "",
                        PriceTotal = br.PriceTotal
                    }).ToList();
                    
                    var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                        ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                        : null;
                    
                    var emailBody = _emailTemplateService.GenerateBookingConfirmationEmail(
                        guestName: guestName,
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
                        to: guestEmail,
                        subject: $"Confirmação de Reserva - {hotel.Name}",
                        body: emailBody,
                        isHtml: true,
                        cc: null,
                        bcc: null);
                    
                    _logger.LogInformation("E-mail de confirmação de reserva enviado para {Email}, Booking: {BookingCode}", 
                        guestEmail, bookingWithDetails.Code);
                }
            }
            catch (Exception emailEx)
            {
                // Não falhar a criação da reserva se o e-mail falhar
                _logger.LogWarning(emailEx, "Erro ao enviar e-mail de confirmação de reserva para Booking: {BookingId}", 
                    createdBooking.Id);
            }

            // Enviar e-mail de notificação para o hotel
            try
            {
                var hotelEmail = hotel.Email;
                if (!string.IsNullOrWhiteSpace(hotelEmail))
                {
                    var guestPii = bookingWithDetails.MainGuest?.GuestPii;
                    var guestName = guestPii?.FullName ?? "Hóspede";
                    var guestEmail = guestPii?.Email ?? "";
                    var guestPhone = guestPii?.PhoneE164;
                    var nights = (bookingWithDetails.CheckOutDate - bookingWithDetails.CheckInDate).Days;
                    var rooms = bookingWithDetails.BookingRooms.Select(br => new BookingRoomInfo
                    {
                        RoomNumber = br.Room?.RoomNumber ?? "",
                        RoomTypeName = br.RoomType?.Name ?? "",
                        PriceTotal = br.PriceTotal
                    }).ToList();

                    var emailBody = _emailTemplateService.GenerateHotelBookingNotificationEmail(
                        hotelName: hotel.Name,
                        bookingCode: bookingWithDetails.Code,
                        guestName: guestName,
                        guestEmail: guestEmail,
                        guestPhone: guestPhone,
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
                        to: hotelEmail,
                        subject: $"Nova Reserva Recebida - {bookingWithDetails.Code}",
                        body: emailBody,
                        isHtml: true,
                        cc: null,
                        bcc: null);

                    _logger.LogInformation("E-mail de notificação de reserva enviado para hotel {HotelEmail}, Booking: {BookingCode}",
                        hotelEmail, bookingWithDetails.Code);
                }
            }
            catch (Exception emailEx)
            {
                // Não falhar a criação da reserva se o e-mail falhar
                _logger.LogWarning(emailEx, "Erro ao enviar e-mail de notificação de reserva para hotel, Booking: {BookingId}",
                    createdBooking.Id);
            }
        }
        
        return MapToResponse(createdBooking);
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
        
        // _context.BookingStatusHistories.Add(history);
        // await _context.SaveChangesAsync();
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

        // Enviar e-mails de cancelamento
        try
        {
            var hotel = booking.Hotel;
            var guestPii = booking.MainGuest?.GuestPii;
            
            // E-mail para o hóspede
            if (guestPii != null && !string.IsNullOrWhiteSpace(guestPii.Email))
            {
                var emailBody = _emailTemplateService.GenerateBookingCancellationEmail(
                    guestName: guestPii.FullName ?? "Hóspede",
                    hotelName: hotel?.Name ?? "Hotel",
                    bookingCode: booking.Code,
                    checkInDate: booking.CheckInDate,
                    checkOutDate: booking.CheckOutDate,
                    totalAmount: booking.TotalAmount,
                    currency: booking.Currency,
                    reason: reason);

                await _emailService.SendEmailAsync(
                    to: guestPii.Email,
                    subject: $"Reserva Cancelada - {hotel?.Name ?? "Hotel"}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de cancelamento enviado para hóspede {Email}, Booking: {BookingCode}",
                    guestPii.Email, booking.Code);
            }

            // E-mail para o hotel
            if (hotel != null && !string.IsNullOrWhiteSpace(hotel.Email))
            {
                var guestPiiForHotel = booking.MainGuest?.GuestPii;
                var emailBody = _emailTemplateService.GenerateHotelBookingCancellationEmail(
                    hotelName: hotel.Name,
                    bookingCode: booking.Code,
                    guestName: guestPiiForHotel?.FullName ?? "Hóspede",
                    guestEmail: guestPiiForHotel?.Email ?? "",
                    checkInDate: booking.CheckInDate,
                    checkOutDate: booking.CheckOutDate,
                    totalAmount: booking.TotalAmount,
                    currency: booking.Currency,
                    reason: reason);

                await _emailService.SendEmailAsync(
                    to: hotel.Email,
                    subject: $"Reserva Cancelada - {booking.Code}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de cancelamento enviado para hotel {HotelEmail}, Booking: {BookingCode}",
                    hotel.Email, booking.Code);
            }
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Erro ao enviar e-mails de cancelamento para Booking: {BookingId}", booking.Id);
        }

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

        // Enviar e-mail de confirmação para o hóspede
        try
        {
            var hotel = booking.Hotel;
            var guestPii = booking.MainGuest?.GuestPii;
            
            if (guestPii != null && !string.IsNullOrWhiteSpace(guestPii.Email) && hotel != null)
            {
                var nights = (booking.CheckOutDate - booking.CheckInDate).Days;
                var rooms = booking.BookingRooms.Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room?.RoomNumber ?? "",
                    RoomTypeName = br.RoomType?.Name ?? "",
                    PriceTotal = br.PriceTotal
                }).ToList();

                var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                    ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                    : null;

                var emailBody = _emailTemplateService.GenerateBookingConfirmedEmail(
                    guestName: guestPii.FullName ?? "Hóspede",
                    hotelName: hotel.Name,
                    bookingCode: booking.Code,
                    checkInDate: booking.CheckInDate,
                    checkOutDate: booking.CheckOutDate,
                    nights: nights,
                    rooms: rooms,
                    hotelAddress: hotelAddress,
                    hotelPhone: hotel.PhoneE164);

                await _emailService.SendEmailAsync(
                    to: guestPii.Email,
                    subject: $"Reserva Confirmada - {hotel.Name}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de confirmação de reserva enviado para {Email}, Booking: {BookingCode}",
                    guestPii.Email, booking.Code);
            }
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Erro ao enviar e-mail de confirmação de reserva para Booking: {BookingId}", booking.Id);
        }

        return true;
    }

    public async Task<bool> CheckInAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CHECKED_IN";
        booking.UpdatedAt = DateTime.UtcNow;

        // Atualizar status do quarto para ocupado
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

        // Enviar e-mail de confirmação de check-in para o hóspede
        try
        {
            var hotel = booking.Hotel;
            var guestPii = booking.MainGuest?.GuestPii;
            
            if (guestPii != null && !string.IsNullOrWhiteSpace(guestPii.Email) && hotel != null)
            {
                var rooms = booking.BookingRooms.Select(br => new BookingRoomInfo
                {
                    RoomNumber = br.Room?.RoomNumber ?? "",
                    RoomTypeName = br.RoomType?.Name ?? "",
                    PriceTotal = br.PriceTotal
                }).ToList();

                var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                    ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                    : null;

                var emailBody = _emailTemplateService.GenerateCheckInConfirmationEmail(
                    guestName: guestPii.FullName ?? "Hóspede",
                    hotelName: hotel.Name,
                    bookingCode: booking.Code,
                    rooms: rooms,
                    checkOutDate: booking.CheckOutDate,
                    hotelAddress: hotelAddress,
                    hotelPhone: hotel.PhoneE164);

                await _emailService.SendEmailAsync(
                    to: guestPii.Email,
                    subject: $"Check-in Realizado - {hotel.Name}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de check-in enviado para {Email}, Booking: {BookingCode}",
                    guestPii.Email, booking.Code);
            }
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Erro ao enviar e-mail de check-in para Booking: {BookingId}", booking.Id);
        }

        return true;
    }

    public async Task<bool> CheckOutAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CHECKED_OUT";
        booking.UpdatedAt = DateTime.UtcNow;

        // Liberar quartos
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

        // Enviar e-mail de confirmação de check-out para o hóspede
        try
        {
            var hotel = booking.Hotel;
            var guestPii = booking.MainGuest?.GuestPii;
            
            if (guestPii != null && !string.IsNullOrWhiteSpace(guestPii.Email) && hotel != null)
            {
                var nights = (booking.CheckOutDate - booking.CheckInDate).Days;
                var hotelAddress = !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                    ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                    : null;

                var emailBody = _emailTemplateService.GenerateCheckOutConfirmationEmail(
                    guestName: guestPii.FullName ?? "Hóspede",
                    hotelName: hotel.Name,
                    bookingCode: booking.Code,
                    checkInDate: booking.CheckInDate,
                    checkOutDate: booking.CheckOutDate,
                    nights: nights,
                    totalAmount: booking.TotalAmount,
                    currency: booking.Currency,
                    hotelAddress: hotelAddress,
                    hotelPhone: hotel.PhoneE164);

                await _emailService.SendEmailAsync(
                    to: guestPii.Email,
                    subject: $"Check-out Realizado - {hotel.Name}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de check-out enviado para {Email}, Booking: {BookingCode}",
                    guestPii.Email, booking.Code);

                // Enviar e-mail de agradecimento pós-estadia após 1 hora
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromHours(1));
                    try
                    {
                        var thankYouEmailBody = _emailTemplateService.GeneratePostStayThankYouEmail(
                            guestName: guestPii.FullName ?? "Hóspede",
                            hotelName: hotel.Name,
                            bookingCode: booking.Code,
                            checkInDate: booking.CheckInDate,
                            checkOutDate: booking.CheckOutDate,
                            nights: nights,
                            hotelAddress: hotelAddress,
                            hotelPhone: hotel.PhoneE164);

                        await _emailService.SendEmailAsync(
                            to: guestPii.Email,
                            subject: $"Obrigado pela sua estadia - {hotel.Name}",
                            body: thankYouEmailBody,
                            isHtml: true,
                            cc: null,
                            bcc: null);

                        _logger.LogInformation("E-mail de agradecimento pós-estadia enviado para {Email}, Booking: {BookingCode}",
                            guestPii.Email, booking.Code);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erro ao enviar e-mail de agradecimento pós-estadia para Booking: {BookingId}",
                            booking.Id);
                    }
                });
            }
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Erro ao enviar e-mail de check-out para Booking: {BookingId}", booking.Id);
        }

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

        // Verificar conflitos com blocos de manutenção
        // Note: Você precisará injetar ApplicationDbContext para isso
        // var hasMaintenance = await _context.MaintenanceBlocks
        //     .AnyAsync(mb => mb.RoomId == roomId && mb.Status == "ACTIVE" 
        //         && mb.StartDate <= checkOut && mb.EndDate >= checkIn);
        
        // Verificar conflitos com reservas existentes
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
        // Calcular total baseado nos quartos solicitados
        var days = (request.CheckOutDate - request.CheckInDate).Days;
        return request.BookingRooms.Sum(br => br.PriceTotal) * days;
    }
}

