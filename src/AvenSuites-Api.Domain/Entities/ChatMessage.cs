using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid SessionId { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Direction { get; set; } = string.Empty;
    
    [MaxLength(120)]
    public string? MessageIdExt { get; set; }
    
    public string? ContentText { get; set; }
    
    public string? RawPayload { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual ChatSession Session { get; set; } = null!;
}

