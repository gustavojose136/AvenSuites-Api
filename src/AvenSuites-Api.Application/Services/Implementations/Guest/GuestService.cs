using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Guest;

public class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepository;
    private readonly IGuestPiiRepository _guestPiiRepository;
    private readonly IHotelRepository _hotelRepository;

    public GuestService(
        IGuestRepository guestRepository,
        IGuestPiiRepository guestPiiRepository,
        IHotelRepository hotelRepository)
    {
        _guestRepository = guestRepository;
        _guestPiiRepository = guestPiiRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<GuestResponse?> CreateGuestAsync(GuestCreateRequest request)
    {
        // Validar hotel
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
        if (hotel == null)
            return null;

        // Criar dados PII
        var guestPii = new GuestPii
        {
            GuestId = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PhoneE164 = request.PhoneE164,
            DocumentType = request.DocumentType,
            DocumentPlain = request.DocumentPlain,
            BirthDate = request.BirthDate,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            CountryCode = request.CountryCode,
            DocumentKeyVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _guestPiiRepository.AddOrUpdateAsync(guestPii);

        // Criar hóspede
        var guest = new AvenSuitesApi.Domain.Entities.Guest
        {
            Id = Guid.NewGuid(),
            GuestPii = guestPii,
            HotelId = request.HotelId,
            MarketingConsent = request.MarketingConsent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdGuest = await _guestRepository.AddAsync(guest);

        return MapToResponse(createdGuest);
    }

    public async Task<GuestResponse?> GetGuestByIdAsync(Guid id)
    {
        var guest = await _guestRepository.GetByIdWithPiiAsync(id);
        if (guest == null)
            return null;

        return MapToResponse(guest);
    }

    public async Task<IEnumerable<GuestResponse>> GetGuestsByHotelAsync(Guid hotelId)
    {
        var guests = await _guestRepository.GetByHotelIdAsync(hotelId);
        return guests.Select(g => MapToResponse(g));
    }

    public async Task<GuestResponse?> UpdateGuestAsync(Guid id, GuestCreateRequest request)
    {
        var guest = await _guestRepository.GetByIdWithPiiAsync(id);
        if (guest == null)
            return null;

        guest.MarketingConsent = request.MarketingConsent;
        guest.UpdatedAt = DateTime.UtcNow;

        var updatedGuest = await _guestRepository.UpdateAsync(guest);

        // Atualizar PII
        if (guest.GuestPii != null)
        {
            guest.GuestPii.FullName = request.FullName;
            guest.GuestPii.Email = request.Email;
            guest.GuestPii.PhoneE164 = request.PhoneE164;
            guest.GuestPii.DocumentType = request.DocumentType;
            guest.GuestPii.DocumentPlain = request.DocumentPlain;
            guest.GuestPii.BirthDate = request.BirthDate;
            guest.GuestPii.AddressLine1 = request.AddressLine1;
            guest.GuestPii.AddressLine2 = request.AddressLine2;
            guest.GuestPii.City = request.City;
            guest.GuestPii.State = request.State;
            guest.GuestPii.PostalCode = request.PostalCode;
            guest.GuestPii.CountryCode = request.CountryCode;
            guest.GuestPii.UpdatedAt = DateTime.UtcNow;

            await _guestPiiRepository.AddOrUpdateAsync(guest.GuestPii);
        }

        return MapToResponse(updatedGuest);
    }

    public async Task<bool> DeleteGuestAsync(Guid id)
    {
        await _guestRepository.DeleteAsync(id);
        return true;
    }

    private static GuestResponse MapToResponse(AvenSuitesApi.Domain.Entities.Guest guest)
    {
        return new GuestResponse
        {
            Id = guest.Id,
            HotelId = guest.HotelId,
            FullName = guest.GuestPii?.FullName ?? string.Empty,
            Email = guest.GuestPii?.Email,
            PhoneE164 = guest.GuestPii?.PhoneE164,
            MarketingConsent = guest.MarketingConsent,
            CreatedAt = guest.CreatedAt,
            UpdatedAt = guest.UpdatedAt
        };
    }
}

