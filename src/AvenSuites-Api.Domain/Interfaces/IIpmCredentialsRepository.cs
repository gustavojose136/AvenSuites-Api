using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IIpmCredentialsRepository
{
    Task<IpmCredentials?> GetByHotelIdAsync(Guid hotelId);
    Task<IpmCredentials> AddAsync(IpmCredentials credentials);
    Task<IpmCredentials> UpdateAsync(IpmCredentials credentials);
    Task DeleteAsync(Guid id);
}

