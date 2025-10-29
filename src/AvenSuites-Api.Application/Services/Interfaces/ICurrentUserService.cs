namespace AvenSuitesApi.Application.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
    Guid? GetUserHotelId();
    List<string> GetUserRoles();
    bool IsAdmin();
    bool IsHotelAdmin();
    bool HasAccessToHotel(Guid hotelId);
}

