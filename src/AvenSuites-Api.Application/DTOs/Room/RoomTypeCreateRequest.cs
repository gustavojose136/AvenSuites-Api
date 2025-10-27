using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomTypeCreateRequest
{
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public short CapacityAdults { get; set; } = 2;
    
    public short CapacityChildren { get; set; } = 0;
    
    [Required]
    public decimal BasePrice { get; set; }
    
    public bool Active { get; set; } = true;
    
    public List<Guid>? AmenityIds { get; set; }
}

