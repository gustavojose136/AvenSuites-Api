using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Usuário não autenticado");
        
        return userId;
    }

    public string GetUserEmail()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAccessException("Email do usuário não encontrado");
        
        return email;
    }

    public Guid? GetUserHotelId()
    {
        var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("HotelId")?.Value;
        
        if (string.IsNullOrEmpty(hotelIdClaim))
            return null;
        
        return Guid.TryParse(hotelIdClaim, out var hotelId) ? hotelId : null;
    }

    public List<string> GetUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
        
        return roles ?? new List<string>();
    }

    public bool IsAdmin()
    {
        return GetUserRoles().Contains("Admin");
    }

    public bool IsHotelAdmin()
    {
        return GetUserRoles().Contains("Hotel-Admin");
    }

    public bool HasAccessToHotel(Guid hotelId)
    {
        // Admin tem acesso a todos os hotéis
        if (IsAdmin())
            return true;
        
        // Hotel-Admin só tem acesso ao próprio hotel
        if (IsHotelAdmin())
        {
            var userHotelId = GetUserHotelId();
            return userHotelId.HasValue && userHotelId.Value == hotelId;
        }
        
        return false;
    }
}

