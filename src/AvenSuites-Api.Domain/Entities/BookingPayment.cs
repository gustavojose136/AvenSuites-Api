using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class BookingPayment
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BookingId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Method { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "PENDING";
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "BRL";
    
    [MaxLength(120)]
    public string? TransactionId { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
}

