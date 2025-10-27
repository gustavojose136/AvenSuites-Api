using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class ErpIntegrationLog
{
    public Guid Id { get; set; }
    
    public Guid? BookingId { get; set; }
    
    public Guid? InvoiceId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Endpoint { get; set; } = string.Empty;
    
    [Required]
    public bool Success { get; set; }
    
    [MaxLength(500)]
    public string? ErrorMessage { get; set; }
    
    public string? RequestJson { get; set; }
    
    public string? ResponseJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Booking? Booking { get; set; }
    public virtual Invoice? Invoice { get; set; }
}

