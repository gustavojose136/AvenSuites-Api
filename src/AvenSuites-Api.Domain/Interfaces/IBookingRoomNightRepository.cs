using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IBookingRoomNightRepository
{
    Task<BookingRoomNight?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingRoomNight>> GetByBookingRoomIdAsync(Guid bookingRoomId);
    Task<IEnumerable<BookingRoomNight>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<BookingRoomNight>> GetByRoomIdAndDateRangeAsync(Guid roomId, DateTime startDate, DateTime endDate);
    Task<BookingRoomNight> AddAsync(BookingRoomNight bookingRoomNight);
    Task AddRangeAsync(IEnumerable<BookingRoomNight> bookingRoomNights);
    Task<BookingRoomNight> UpdateAsync(BookingRoomNight bookingRoomNight);
    Task DeleteAsync(Guid id);
    Task DeleteByBookingRoomIdAsync(Guid bookingRoomId);
    Task<bool> HasConflictAsync(Guid roomId, DateTime startDate, DateTime endDate, Guid? excludeBookingRoomId = null);
    Task<bool> ExistsAsync(Guid id);
}





