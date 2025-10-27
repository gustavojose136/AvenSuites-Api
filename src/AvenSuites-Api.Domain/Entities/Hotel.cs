using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(150)]
    public string? TradeName { get; set; }
    
    [MaxLength(18)]
    public string? Cnpj { get; set; }
    
    [MaxLength(320)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? PhoneE164 { get; set; }
    
    [MaxLength(64)]
    public string Timezone { get; set; } = "America/Sao_Paulo";
    
    [MaxLength(160)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(160)]
    public string? AddressLine2 { get; set; }
    
    [MaxLength(120)]
    public string? City { get; set; }
    
    [MaxLength(60)]
    public string? State { get; set; }
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [MaxLength(2)]
    public string CountryCode { get; set; } = "BR";
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();
    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public virtual ICollection<HotelKey> HotelKeys { get; set; } = new List<HotelKey>();
}

