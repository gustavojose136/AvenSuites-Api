using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string RoomNumber { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Floor { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "ACTIVE";
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual RoomType RoomType { get; set; } = null!;
    public virtual ICollection<MaintenanceBlock> MaintenanceBlocks { get; set; } = new List<MaintenanceBlock>();
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    public virtual ICollection<BookingRoomNight> BookingRoomNights { get; set; } = new List<BookingRoomNight>();
}

