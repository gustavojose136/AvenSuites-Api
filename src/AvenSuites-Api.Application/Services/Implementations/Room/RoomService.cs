using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Room;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IRoomTypeRepository roomTypeRepository)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<RoomResponse?> CreateRoomAsync(RoomCreateRequest request)
    {
        // Validar hotel
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
        if (hotel == null)
            return null;

        // Validar tipo de quarto
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId);
        if (roomType == null)
            return null;

        // Verificar se o número já existe
        var isUnique = await _roomRepository.IsRoomNumberUniqueAsync(request.HotelId, request.RoomNumber);
        if (!isUnique)
            return null;

        var room = new AvenSuitesApi.Domain.Entities.Room
        {
            Id = Guid.NewGuid(),
            HotelId = request.HotelId,
            RoomTypeId = request.RoomTypeId,
            RoomNumber = request.RoomNumber,
            Floor = request.Floor,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdRoom = await _roomRepository.AddAsync(room);
        return MapToResponse(createdRoom);
    }

    public async Task<RoomResponse?> GetRoomByIdAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdWithDetailsAsync(id);
        if (room == null)
            return null;

        return MapToResponse(room);
    }

    public async Task<IEnumerable<RoomResponse>> GetRoomsByHotelAsync(Guid hotelId, string? status = null, DateTime? checkInDate = null, DateTime? checkOutDate = null, short? guests = null)
    {
        IEnumerable<AvenSuitesApi.Domain.Entities.Room> rooms;

        // Se datas foram fornecidas, filtrar por disponibilidade
        if (checkInDate.HasValue && checkOutDate.HasValue)
        {
            rooms = await _roomRepository.GetAvailableRoomsForPeriodAsync(hotelId, checkInDate.Value, checkOutDate.Value);
            
            // Aplicar filtro de status se fornecido
            if (!string.IsNullOrEmpty(status))
            {
                rooms = rooms.Where(r => r.Status == status);
            }
        }
        else
        {
            // Lógica original sem filtro de data
            rooms = string.IsNullOrEmpty(status)
                ? await _roomRepository.GetByHotelIdAsync(hotelId)
                : await _roomRepository.GetByStatusAsync(hotelId, status);
        }

        // Se guests e datas foram fornecidos, calcular preços
        if (guests.HasValue && guests.Value > 0 && checkInDate.HasValue && checkOutDate.HasValue)
        {
            var responses = new List<RoomResponse>();
            
            foreach (var room in rooms)
            {
                // Buscar RoomType com preços de ocupação
                var roomType = await _roomTypeRepository.GetByIdWithOccupancyPricesAsync(room.RoomTypeId);
                if (roomType == null)
                    continue;

                // Calcular preço por ocupação
                var pricePerNight = GetPriceForOccupancy(roomType, guests.Value);
                var totalPrice = CalculateTotalPrice(pricePerNight, checkInDate.Value, checkOutDate.Value);
                
                var response = MapToResponse(room);
                response.PricePerNight = pricePerNight;
                response.TotalPrice = totalPrice;
                
                responses.Add(response);
            }
            
            return responses;
        }

        return rooms.Select(MapToResponse);
    }

    public async Task<RoomResponse?> UpdateRoomAsync(Guid id, RoomUpdateRequest request)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
            return null;

        if (request.RoomTypeId.HasValue)
            room.RoomTypeId = request.RoomTypeId.Value;

        if (!string.IsNullOrEmpty(request.RoomNumber))
            room.RoomNumber = request.RoomNumber;

        if (!string.IsNullOrEmpty(request.Floor))
            room.Floor = request.Floor;

        if (!string.IsNullOrEmpty(request.Status))
            room.Status = request.Status;

        room.UpdatedAt = DateTime.UtcNow;

        var updatedRoom = await _roomRepository.UpdateAsync(room);
        return MapToResponse(updatedRoom);
    }

    public async Task<bool> DeleteRoomAsync(Guid id)
    {
        await _roomRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<RoomAvailabilityResponse>> CheckAvailabilityAsync(RoomAvailabilityRequest request)
    {
        // Usar método melhorado que verifica disponibilidade real
        var availableRooms = await _roomRepository.GetAvailableRoomsForPeriodAsync(
            request.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            request.RoomTypeId);

        // Filtrar por capacidade
        availableRooms = availableRooms.Where(r => 
            r.RoomType.CapacityAdults >= request.Adults &&
            r.RoomType.CapacityChildren >= request.Children);

        var responses = new List<RoomAvailabilityResponse>();
        
        foreach (var room in availableRooms)
        {
            // Buscar RoomType com preços de ocupação
            var roomType = await _roomTypeRepository.GetByIdWithOccupancyPricesAsync(room.RoomTypeId);
            if (roomType == null)
                continue;

            // Calcular ocupação total (adultos + crianças)
            var totalOccupancy = (short)(request.Adults + request.Children);
            
            // Buscar preço por ocupação ou usar BasePrice como fallback
            var pricePerNight = GetPriceForOccupancy(roomType, totalOccupancy);
            
            responses.Add(new RoomAvailabilityResponse
            {
                RoomId = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType.Name,
                CapacityAdults = room.RoomType.CapacityAdults,
                CapacityChildren = room.RoomType.CapacityChildren,
                BasePrice = roomType.BasePrice,
                TotalPrice = CalculateTotalPrice(pricePerNight, request.CheckInDate, request.CheckOutDate),
                IsAvailable = true
            });
        }

        return responses;
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null || room.Status != "ACTIVE")
            return false;

        // Implementar lógica completa de verificação
        return true;
    }

    private static RoomResponse MapToResponse(AvenSuitesApi.Domain.Entities.Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            HotelId = room.HotelId,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            Status = room.Status,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
            RoomType = new RoomTypeSummaryResponse
            {
                Id = room.RoomType.Id,
                Code = room.RoomType.Code,
                Name = room.RoomType.Name,
                Description = room.RoomType.Description,
                CapacityAdults = room.RoomType.CapacityAdults,
                CapacityChildren = room.RoomType.CapacityChildren,
                BasePrice = room.RoomType.BasePrice,
                Active = room.RoomType.Active
            }
        };
    }

    /// <summary>
    /// Obtém o preço por noite baseado na ocupação.
    /// Busca primeiro um preço específico para a ocupação no RoomTypeOccupancyPrice.
    /// Se não encontrar, usa o BasePrice do RoomType.
    /// </summary>
    private static decimal GetPriceForOccupancy(RoomType roomType, short occupancy)
    {
        // Buscar preço específico para a ocupação
        var occupancyPrice = roomType.OccupancyPrices?
            .FirstOrDefault(op => op.Occupancy == occupancy);

        // Se encontrar preço específico, usar ele; caso contrário, usar BasePrice
        return occupancyPrice?.PricePerNight ?? roomType.BasePrice;
    }

    private static decimal CalculateTotalPrice(decimal pricePerNight, DateTime checkIn, DateTime checkOut)
    {
        var nights = (checkOut - checkIn).Days;
        return pricePerNight * nights;
    }
}

