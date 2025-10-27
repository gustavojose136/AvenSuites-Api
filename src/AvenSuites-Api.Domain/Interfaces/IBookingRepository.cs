using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<Booking?> GetByIdWithDetailsAsync(Guid id);
    Task<Booking?> GetByCodeAsync(Guid hotelId, string code);
    Task<IEnumerable<Booking>> GetByHotelIdAsync(Guid hotelId);
    Task<IEnumerable<Booking>> GetByHotelIdAndDatesAsync(Guid hotelId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Booking>> GetByGuestIdAsync(Guid guestId);
    Task<IEnumerable<Booking>> GetByStatusAsync(Guid hotelId, string status);
    Task<Booking> AddAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsCodeUniqueAsync(Guid hotelId, string code);
}

