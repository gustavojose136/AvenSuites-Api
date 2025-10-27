using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Booking;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRatePlanRepository _ratePlanRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IGuestRepository guestRepository,
        IRoomRepository roomRepository,
        IRatePlanRepository ratePlanRepository)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _guestRepository = guestRepository;
        _roomRepository = roomRepository;
        _ratePlanRepository = ratePlanRepository;
    }

    public async Task<BookingResponse?> CreateBookingAsync(BookingCreateRequest request)
    {
        // Validar hotel
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
        if (hotel == null)
            return null;

        // Validar hóspede principal
        var mainGuest = await _guestRepository.GetByIdAsync(request.MainGuestId);
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
            MainGuestId = request.MainGuestId,
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
            
            // Adicionar ao contexto - você precisará injetar ApplicationDbContext
            // _context.BookingRooms.Add(bookingRoom);
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
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        var oldStatus = booking.Status;
        booking.Status = "CANCELLED";
        booking.Notes = string.IsNullOrEmpty(reason) ? booking.Notes : reason;
        booking.UpdatedAt = DateTime.UtcNow;

        await RegisterStatusChangeAsync(booking.Id, oldStatus, "CANCELLED", reason);
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> ConfirmBookingAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        var oldStatus = booking.Status;
        booking.Status = "CONFIRMED";
        booking.UpdatedAt = DateTime.UtcNow;

        await RegisterStatusChangeAsync(booking.Id, oldStatus, "CONFIRMED", "Reserva confirmada via API");
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    private static BookingResponse MapToResponse(AvenSuitesApi.Domain.Entities.Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
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
                RoomTypeName = br.RoomType.Name,
                PriceTotal = br.PriceTotal,
                Notes = br.Notes
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

