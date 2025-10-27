using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class IntegrationEventOutbox
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(80)]
    public string AggregateType { get; set; } = string.Empty;
    
    [Required]
    public Guid AggregateId { get; set; }
    
    [Required]
    [MaxLength(80)]
    public string EventType { get; set; } = string.Empty;
    
    [Required]
    public string PayloadJson { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string Status { get; set; } = "PENDING";
    
    public int Attempts { get; set; } = 0;
    
    [MaxLength(500)]
    public string? LastError { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
}

