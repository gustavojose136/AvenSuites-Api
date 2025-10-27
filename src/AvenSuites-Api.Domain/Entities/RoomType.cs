using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class RoomType
{
    public Guid Id { get; set; }
    
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
    [Range(0, 999999999999.99)]
    public decimal BasePrice { get; set; } = 0.00m;
    
    public bool Active { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
}

