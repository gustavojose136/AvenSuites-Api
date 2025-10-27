using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class RatePlan
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "BRL";
    
    [Required]
    [MaxLength(30)]
    public string MealPlan { get; set; } = "ROOM_ONLY";
    
    public string? CancellationPolicy { get; set; }
    
    public bool Active { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual RoomType RoomType { get; set; } = null!;
    public virtual ICollection<RatePlanPrice> Prices { get; set; } = new List<RatePlanPrice>();
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
}

