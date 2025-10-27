using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class NotificationTemplate
{
    [Key]
    [Required]
    [MaxLength(80)]
    public string TemplateKey { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    public string Channel { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? SubjectTemplate { get; set; }
    
    public string? BodyTemplate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}

