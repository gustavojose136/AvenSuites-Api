using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IRatePlanRepository
{
    Task<RatePlan?> GetByIdAsync(Guid id);
    Task<RatePlan?> GetByIdWithPricesAsync(Guid id);
    Task<IEnumerable<RatePlan>> GetByHotelIdAsync(Guid hotelId);
    Task<IEnumerable<RatePlan>> GetByRoomTypeIdAsync(Guid roomTypeId);
    Task<IEnumerable<RatePlan>> GetActiveByHotelIdAsync(Guid hotelId);
    Task<RatePlan> AddAsync(RatePlan ratePlan);
    Task<RatePlan> UpdateAsync(RatePlan ratePlan);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

