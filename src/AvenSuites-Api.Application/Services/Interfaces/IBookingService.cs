using AvenSuitesApi.Application.DTOs.Booking;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse?> CreateBookingAsync(BookingCreateRequest request);
    Task<BookingResponse?> GetBookingByIdAsync(Guid id);
    Task<BookingResponse?> GetBookingByCodeAsync(Guid hotelId, string code);
    Task<IEnumerable<BookingResponse>> GetBookingsByHotelAsync(Guid hotelId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<BookingResponse>> GetBookingsByGuestAsync(Guid guestId);
    Task<BookingResponse?> UpdateBookingAsync(Guid id, BookingUpdateRequest request);
    Task<bool> CancelBookingAsync(Guid id, string? reason = null);
    Task<bool> ConfirmBookingAsync(Guid id);
}

