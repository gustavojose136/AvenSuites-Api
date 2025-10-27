namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public RoomTypeSummaryResponse? RoomType { get; set; }
}

public class RoomTypeSummaryResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short CapacityAdults { get; set; }
    public short CapacityChildren { get; set; }
    public decimal BasePrice { get; set; }
    public bool Active { get; set; }
}

