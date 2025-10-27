using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IGuestRepository
{
    Task<Guest?> GetByIdAsync(Guid id);
    Task<Guest?> GetByIdWithPiiAsync(Guid id);
    Task<IEnumerable<Guest>> GetByHotelIdAsync(Guid hotelId);
    Task<Guest> AddAsync(Guest guest);
    Task<Guest> UpdateAsync(Guest guest);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

