using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Room>> GetByHotelIdAsync(Guid hotelId);
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime startDate, DateTime endDate, Guid? roomTypeId = null);
    Task<IEnumerable<Room>> GetByStatusAsync(Guid hotelId, string status);
    Task<Room> AddAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsRoomNumberUniqueAsync(Guid hotelId, string roomNumber, Guid? excludeRoomId = null);
}

