using AvenSuitesApi.Application.DTOs.Hotel;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IHotelService
{
    Task<HotelResponse?> CreateHotelAsync(HotelCreateRequest request);
    Task<HotelResponse?> GetHotelByIdAsync(Guid id);
    Task<HotelResponse?> GetHotelByCnpjAsync(string cnpj);
    Task<IEnumerable<HotelResponse>> GetAllHotelsAsync();
    Task<HotelResponse?> UpdateHotelAsync(Guid id, HotelCreateRequest request);
    Task<bool> DeleteHotelAsync(Guid id);
}

