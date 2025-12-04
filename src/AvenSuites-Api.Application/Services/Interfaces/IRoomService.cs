using AvenSuitesApi.Application.DTOs.Room;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IRoomService
{
    Task<RoomResponse?> CreateRoomAsync(RoomCreateRequest request);
    Task<RoomResponse?> GetRoomByIdAsync(Guid id);
    Task<IEnumerable<RoomResponse>> GetRoomsByHotelAsync(Guid hotelId, string? status = null, DateTime? checkInDate = null, DateTime? checkOutDate = null, short? guests = null);
    Task<RoomResponse?> UpdateRoomAsync(Guid id, RoomUpdateRequest request);
    Task<bool> DeleteRoomAsync(Guid id);
    Task<IEnumerable<RoomAvailabilityResponse>> CheckAvailabilityAsync(RoomAvailabilityRequest request);
    Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate);
}

