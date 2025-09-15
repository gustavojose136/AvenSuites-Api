using AvenSuitesApi.Application.DTOs;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> RegisterAsync(RegisterRequest request);
    Task<bool> ValidatePasswordAsync(string email, string password);
}
