namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomAvailabilityResponse
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public short CapacityAdults { get; set; }
    public short CapacityChildren { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsAvailable { get; set; }
}

