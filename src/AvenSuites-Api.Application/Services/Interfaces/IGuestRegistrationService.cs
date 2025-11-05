using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.DTOs;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IGuestRegistrationService
{
    Task<LoginResponse> RegisterAsync(GuestRegisterRequest request);
    Task<GuestProfileResponse> GetProfileAsync(Guid guestId);
    Task<GuestProfileResponse> UpdateProfileAsync(Guid guestId, GuestRegisterRequest request);
}

