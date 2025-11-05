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

    public Guid? GetUserGuestId()
    {
        var guestIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("GuestId")?.Value;
        
        if (string.IsNullOrEmpty(guestIdClaim))
            return null;
        
        return Guid.TryParse(guestIdClaim, out var guestId) ? guestId : null;
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

    public bool IsGuest()
    {
        return GetUserRoles().Contains("Guest");
    }

    public bool HasAccessToHotel(Guid hotelId)
    {
        // Admin tem acesso a todos os hotéis
        if (IsAdmin() || IsGuest())
            return true;
        
        // Hotel-Admin só tem acesso ao próprio hotel
        if (IsHotelAdmin())
        {
            var userHotelId = GetUserHotelId();
            return userHotelId.HasValue && userHotelId.Value == hotelId;
        }
        
        return false;
    }

    public bool HasAccessToGuest(Guid guestId)
    {
        // Admin tem acesso a todos os guests
        if (IsAdmin())
            return true;
        
        // Hotel-Admin tem acesso a guests do seu hotel (precisa verificar via repository)
        if (IsHotelAdmin())
            return true; // A verificação completa será feita no controller
        
        // Guest só tem acesso aos próprios dados
        if (IsGuest())
        {
            var userGuestId = GetUserGuestId();
            return userGuestId.HasValue && userGuestId.Value == guestId;
        }
        
        return false;
    }
}

