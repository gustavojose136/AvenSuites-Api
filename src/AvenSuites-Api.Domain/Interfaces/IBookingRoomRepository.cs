using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IBookingRoomRepository
{
    Task<BookingRoom?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingRoom>> GetByBookingIdAsync(Guid bookingId);
    Task<IEnumerable<BookingRoom>> GetByRoomIdAsync(Guid roomId);
    Task<BookingRoom> AddAsync(BookingRoom bookingRoom);
    Task<BookingRoom> UpdateAsync(BookingRoom bookingRoom);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

