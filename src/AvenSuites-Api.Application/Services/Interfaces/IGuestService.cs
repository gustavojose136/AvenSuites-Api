using AvenSuitesApi.Application.DTOs.Guest;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IGuestService
{
    Task<GuestResponse?> CreateGuestAsync(GuestCreateRequest request);
    Task<GuestResponse?> GetGuestByIdAsync(Guid id);
    Task<IEnumerable<GuestResponse>> GetGuestsByHotelAsync(Guid hotelId);
    Task<GuestResponse?> UpdateGuestAsync(Guid id, GuestCreateRequest request);
    Task<bool> DeleteGuestAsync(Guid id);
}

