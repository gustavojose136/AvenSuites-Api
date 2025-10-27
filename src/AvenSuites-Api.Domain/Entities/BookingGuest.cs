using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class BookingGuest
{
    [Required]
    public Guid BookingId { get; set; }
    
    [Required]
    public Guid GuestId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "COMPANION";
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual Guest Guest { get; set; } = null!;
}

