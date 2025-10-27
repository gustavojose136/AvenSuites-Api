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
            var isAvailable = await _roomRepository.IsRoomNumberUniqueAsync(request.HotelId, bookingRoom.RoomId.ToString(), null);
            // Implementar lógica completa de verificação de disponibilidade
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
            TotalAmount = 0, // Será calculado
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBooking = await _bookingRepository.AddAsync(booking);
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

        if (!string.IsNullOrEmpty(request.Status))
            booking.Status = request.Status;

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

    public async Task<bool> CancelBookingAsync(Guid id, string? reason = null)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CANCELLED";
        booking.Notes = string.IsNullOrEmpty(reason) ? booking.Notes : reason;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> ConfirmBookingAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.Status = "CONFIRMED";
        booking.UpdatedAt = DateTime.UtcNow;

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
}

