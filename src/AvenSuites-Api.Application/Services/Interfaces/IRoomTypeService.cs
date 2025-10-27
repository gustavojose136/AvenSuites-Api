using AvenSuitesApi.Application.DTOs.Room;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IRoomTypeService
{
    Task<RoomTypeResponse?> CreateRoomTypeAsync(RoomTypeCreateRequest request);
    Task<RoomTypeResponse?> GetRoomTypeByIdAsync(Guid id);
    Task<IEnumerable<RoomTypeResponse>> GetRoomTypesByHotelAsync(Guid hotelId, bool activeOnly = true);
    Task<RoomTypeResponse?> UpdateRoomTypeAsync(Guid id, RoomTypeCreateRequest request);
    Task<bool> DeleteRoomTypeAsync(Guid id);
}

