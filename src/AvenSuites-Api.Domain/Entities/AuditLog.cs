using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    
    public Guid? HotelId { get; set; }
    
    public Guid? ActorUserId { get; set; }
    
    [Required]
    [MaxLength(80)]
    public string EntityName { get; set; } = string.Empty;
    
    [Required]
    public Guid EntityId { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Action { get; set; } = "INSERT";
    
    public string? ChangesJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

