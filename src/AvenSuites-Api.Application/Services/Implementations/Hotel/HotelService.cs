using AvenSuitesApi.Application.DTOs.Hotel;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Hotel;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<HotelResponse?> CreateHotelAsync(HotelCreateRequest request)
    {
        // Verificar se CNPJ já existe
        if (!string.IsNullOrEmpty(request.Cnpj))
        {
            var exists = await _hotelRepository.ExistsByCnpjAsync(request.Cnpj);
            if (exists)
                return null;
        }

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TradeName = request.TradeName,
            Cnpj = request.Cnpj,
            Email = request.Email,
            PhoneE164 = request.PhoneE164,
            Timezone = request.Timezone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            CountryCode = request.CountryCode,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdHotel = await _hotelRepository.AddAsync(hotel);
        return MapToResponse(createdHotel);
    }

    public async Task<HotelResponse?> GetHotelByIdAsync(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel == null)
            return null;

        return MapToResponse(hotel);
    }

    public async Task<HotelResponse?> GetHotelByCnpjAsync(string cnpj)
    {
        var hotel = await _hotelRepository.GetByCnpjAsync(cnpj);
        if (hotel == null)
            return null;

        return MapToResponse(hotel);
    }

    public async Task<IEnumerable<HotelResponse>> GetAllHotelsAsync()
    {
        var hotels = await _hotelRepository.GetAllAsync();
        return hotels.Select(MapToResponse);
    }

    public async Task<HotelResponse?> UpdateHotelAsync(Guid id, HotelCreateRequest request)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel == null)
            return null;

        hotel.Name = request.Name;
        hotel.TradeName = request.TradeName;
        hotel.Cnpj = request.Cnpj;
        hotel.Email = request.Email;
        hotel.PhoneE164 = request.PhoneE164;
        hotel.Timezone = request.Timezone;
        hotel.AddressLine1 = request.AddressLine1;
        hotel.AddressLine2 = request.AddressLine2;
        hotel.City = request.City;
        hotel.State = request.State;
        hotel.PostalCode = request.PostalCode;
        hotel.CountryCode = request.CountryCode;
        hotel.UpdatedAt = DateTime.UtcNow;

        var updatedHotel = await _hotelRepository.UpdateAsync(hotel);
        return MapToResponse(updatedHotel);
    }

    public async Task<bool> DeleteHotelAsync(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel == null)
            return false;

        hotel.Status = "INACTIVE";
        hotel.UpdatedAt = DateTime.UtcNow;

        await _hotelRepository.UpdateAsync(hotel);
        return true;
    }

    private static HotelResponse MapToResponse(AvenSuitesApi.Domain.Entities.Hotel hotel)
    {
        return new HotelResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            TradeName = hotel.TradeName,
            Cnpj = hotel.Cnpj,
            Email = hotel.Email,
            PhoneE164 = hotel.PhoneE164,
            Timezone = hotel.Timezone,
            AddressLine1 = hotel.AddressLine1,
            AddressLine2 = hotel.AddressLine2,
            City = hotel.City,
            State = hotel.State,
            PostalCode = hotel.PostalCode,
            CountryCode = hotel.CountryCode,
            Status = hotel.Status,
            CreatedAt = hotel.CreatedAt,
            UpdatedAt = hotel.UpdatedAt
        };
    }
}

