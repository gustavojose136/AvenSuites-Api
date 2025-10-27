using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(Guid id);
    Task<RoomType?> GetByIdWithAmenitiesAsync(Guid id);
    Task<IEnumerable<RoomType>> GetByHotelIdAsync(Guid hotelId);
    Task<IEnumerable<RoomType>> GetActiveByHotelIdAsync(Guid hotelId);
    Task<RoomType> AddAsync(RoomType roomType);
    Task<RoomType> UpdateAsync(RoomType roomType);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsCodeUniqueAsync(Guid hotelId, string code, Guid? excludeRoomTypeId = null);
}

