using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}
