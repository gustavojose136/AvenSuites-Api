using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "PENDING";
    
    [Required]
    [MaxLength(30)]
    public string Source { get; set; } = string.Empty;
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
    
    public short Adults { get; set; } = 1;
    public short Children { get; set; } = 0;
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "BRL";
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal TotalAmount { get; set; } = 0.00m;
    
    [Required]
    public Guid MainGuestId { get; set; }
    
    [MaxLength(120)]
    public string? ChannelRef { get; set; }
    
    public string? Notes { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual Guest MainGuest { get; set; } = null!;
    public virtual User? Creator { get; set; }
    public virtual ICollection<BookingGuest> BookingGuests { get; set; } = new List<BookingGuest>();
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    public virtual ICollection<BookingPayment> Payments { get; set; } = new List<BookingPayment>();
    public virtual ICollection<BookingStatusHistory> StatusHistory { get; set; } = new List<BookingStatusHistory>();
    public virtual Invoice? Invoice { get; set; }
}

