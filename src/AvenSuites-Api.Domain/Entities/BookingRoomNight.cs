using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class BookingRoomNight
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BookingRoomId { get; set; }
    
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    public DateTime StayDate { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal PriceAmount { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal TaxAmount { get; set; } = 0.00m;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual BookingRoom BookingRoom { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
}

