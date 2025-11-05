namespace AvenSuitesApi.Application.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
    Guid? GetUserHotelId();
    Guid? GetUserGuestId();
    List<string> GetUserRoles();
    bool IsAdmin();
    bool IsHotelAdmin();
    bool IsGuest();
    bool HasAccessToHotel(Guid hotelId);
    bool HasAccessToGuest(Guid guestId);
}

