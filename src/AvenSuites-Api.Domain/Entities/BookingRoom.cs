using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class BookingRoom
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BookingId { get; set; }
    
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    public Guid? RatePlanId { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal PriceTotal { get; set; } = 0.00m;
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
    public virtual RoomType RoomType { get; set; } = null!;
    public virtual RatePlan? RatePlan { get; set; }
    public virtual ICollection<BookingRoomNight> Nights { get; set; } = new List<BookingRoomNight>();
}

