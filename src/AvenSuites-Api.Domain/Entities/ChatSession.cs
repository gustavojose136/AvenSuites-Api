using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    public Guid? GuestId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string WaUserJid { get; set; } = string.Empty;
    
    public string? StateJson { get; set; }
    
    public DateTime? LastInteractionAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual Guest? Guest { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

