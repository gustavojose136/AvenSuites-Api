using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class BookingStatusHistory
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BookingId { get; set; }
    
    [MaxLength(30)]
    public string? OldStatus { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string NewStatus { get; set; } = string.Empty;
    
    public Guid? ChangedBy { get; set; }
    
    public DateTime ChangedAt { get; set; }
    
    [MaxLength(240)]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual User? ChangedByUser { get; set; }
}

