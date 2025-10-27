using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Channel { get; set; } = string.Empty;
    
    [MaxLength(80)]
    public string? TemplateKey { get; set; }
    
    [MaxLength(320)]
    public string? ToAddress { get; set; }
    
    [MaxLength(40)]
    public string? ToWhatsapp { get; set; }
    
    [MaxLength(200)]
    public string? Subject { get; set; }
    
    public string? Body { get; set; }
    
    public Guid? RelatedBookingId { get; set; }
    
    public Guid? RelatedInvoiceId { get; set; }
    
    [MaxLength(120)]
    public string? ProviderMessageId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "QUEUED";
    
    [MaxLength(500)]
    public string? ErrorMessage { get; set; }
    
    public DateTime? SentAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual NotificationTemplate? Template { get; set; }
}

