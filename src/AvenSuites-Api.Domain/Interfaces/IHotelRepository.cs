using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id);
    Task<Hotel?> GetByCnpjAsync(string cnpj);
    Task<IEnumerable<Hotel>> GetAllAsync();
    Task<Hotel> AddAsync(Hotel hotel);
    Task<Hotel> UpdateAsync(Hotel hotel);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByCnpjAsync(string cnpj);
}

