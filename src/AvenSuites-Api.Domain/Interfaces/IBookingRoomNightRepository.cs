using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IBookingRoomNightRepository
{
    Task<IEnumerable<BookingRoomNight>> GetByBookingRoomIdAsync(Guid bookingRoomId);
    Task<IEnumerable<BookingRoomNight>> GetByRoomIdAndDateRangeAsync(Guid roomId, DateTime startDate, DateTime endDate);
    Task<bool> HasConflictAsync(Guid roomId, DateTime startDate, DateTime endDate, Guid? excludeBookingRoomId = null);
    Task<BookingRoomNight> AddAsync(BookingRoomNight bookingRoomNight);
    Task AddRangeAsync(IEnumerable<BookingRoomNight> bookingRoomNights);
    Task DeleteAsync(Guid id);
    Task DeleteByBookingRoomIdAsync(Guid bookingRoomId);
}

