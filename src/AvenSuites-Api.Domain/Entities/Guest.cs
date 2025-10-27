using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Guest
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    public bool MarketingConsent { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual GuestPii? GuestPii { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<BookingGuest> GuestBookings { get; set; } = new List<BookingGuest>();
}

