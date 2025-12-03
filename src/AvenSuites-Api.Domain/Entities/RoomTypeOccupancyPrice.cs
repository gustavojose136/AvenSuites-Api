using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class RoomTypeOccupancyPrice
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    [Required]
    public short Occupancy { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal PricePerNight { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual RoomType RoomType { get; set; } = null!;
}



