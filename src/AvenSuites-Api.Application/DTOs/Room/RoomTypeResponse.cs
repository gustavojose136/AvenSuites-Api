namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomTypeResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short CapacityAdults { get; set; }
    public short CapacityChildren { get; set; }
    public decimal BasePrice { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<AmenitySummaryResponse> Amenities { get; set; } = new();
}

public class AmenitySummaryResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

