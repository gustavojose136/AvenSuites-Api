using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class MaintenanceBlock
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [MaxLength(240)]
    public string? Reason { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "ACTIVE";
    
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Room Room { get; set; } = null!;
}

