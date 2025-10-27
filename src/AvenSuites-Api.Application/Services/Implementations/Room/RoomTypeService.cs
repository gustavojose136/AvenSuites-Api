using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Room;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IAmenityRepository _amenityRepository;

    public RoomTypeService(
        IRoomTypeRepository roomTypeRepository,
        IAmenityRepository amenityRepository)
    {
        _roomTypeRepository = roomTypeRepository;
        _amenityRepository = amenityRepository;
    }

    public async Task<RoomTypeResponse?> CreateRoomTypeAsync(RoomTypeCreateRequest request)
    {
        // Verificar se código já existe
        var existingRoomType = await _roomTypeRepository.GetByIdAsync(Guid.Empty); // Implementar validação correta
        if (existingRoomType != null)
            return null;

        var roomType = new AvenSuitesApi.Domain.Entities.RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = request.HotelId,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CapacityAdults = request.CapacityAdults,
            CapacityChildren = request.CapacityChildren,
            BasePrice = request.BasePrice,
            Active = request.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Adicionar amenidades se fornecidas
        if (request.AmenityIds != null && request.AmenityIds.Any())
        {
            foreach (var amenityId in request.AmenityIds)
            {
                var amenity = await _amenityRepository.GetByIdAsync(amenityId);
                if (amenity != null)
                    roomType.Amenities.Add(amenity);
            }
        }

        var createdRoomType = await _roomTypeRepository.AddAsync(roomType);
        return MapToResponse(createdRoomType);
    }

    public async Task<RoomTypeResponse?> GetRoomTypeByIdAsync(Guid id)
    {
        var roomType = await _roomTypeRepository.GetByIdWithAmenitiesAsync(id);
        if (roomType == null)
            return null;

        return MapToResponse(roomType);
    }

    public async Task<IEnumerable<RoomTypeResponse>> GetRoomTypesByHotelAsync(Guid hotelId, bool activeOnly = true)
    {
        var roomTypes = activeOnly
            ? await _roomTypeRepository.GetActiveByHotelIdAsync(hotelId)
            : await _roomTypeRepository.GetByHotelIdAsync(hotelId);

        return roomTypes.Select(MapToResponse);
    }

    public async Task<RoomTypeResponse?> UpdateRoomTypeAsync(Guid id, RoomTypeCreateRequest request)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(id);
        if (roomType == null)
            return null;

        roomType.Code = request.Code;
        roomType.Name = request.Name;
        roomType.Description = request.Description;
        roomType.CapacityAdults = request.CapacityAdults;
        roomType.CapacityChildren = request.CapacityChildren;
        roomType.BasePrice = request.BasePrice;
        roomType.Active = request.Active;
        roomType.UpdatedAt = DateTime.UtcNow;

        // Atualizar amenidades
        if (request.AmenityIds != null)
        {
            roomType.Amenities.Clear();
            foreach (var amenityId in request.AmenityIds)
            {
                var amenity = await _amenityRepository.GetByIdAsync(amenityId);
                if (amenity != null)
                    roomType.Amenities.Add(amenity);
            }
        }

        var updatedRoomType = await _roomTypeRepository.UpdateAsync(roomType);
        return MapToResponse(updatedRoomType);
    }

    public async Task<bool> DeleteRoomTypeAsync(Guid id)
    {
        await _roomTypeRepository.DeleteAsync(id);
        return true;
    }

    private static RoomTypeResponse MapToResponse(AvenSuitesApi.Domain.Entities.RoomType roomType)
    {
        return new RoomTypeResponse
        {
            Id = roomType.Id,
            HotelId = roomType.HotelId,
            Code = roomType.Code,
            Name = roomType.Name,
            Description = roomType.Description,
            CapacityAdults = roomType.CapacityAdults,
            CapacityChildren = roomType.CapacityChildren,
            BasePrice = roomType.BasePrice,
            Active = roomType.Active,
            CreatedAt = roomType.CreatedAt,
            UpdatedAt = roomType.UpdatedAt,
            Amenities = roomType.Amenities.Select(a => new AmenitySummaryResponse
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name
            }).ToList()
        };
    }
}

