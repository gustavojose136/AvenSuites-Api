using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class IntegrationEventInbox
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string EventIdExt { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(80)]
    public string EventType { get; set; } = string.Empty;
    
    [Required]
    public DateTime ConsumedAt { get; set; }
    
    [MaxLength(64)]
    public string? PayloadHash { get; set; }
}

