using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IAmenityRepository
{
    Task<Amenity?> GetByIdAsync(Guid id);
    Task<Amenity?> GetByCodeAsync(string code);
    Task<IEnumerable<Amenity>> GetAllAsync();
    Task<Amenity> AddAsync(Amenity amenity);
    Task<Amenity> UpdateAsync(Amenity amenity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByCodeAsync(string code);
}

