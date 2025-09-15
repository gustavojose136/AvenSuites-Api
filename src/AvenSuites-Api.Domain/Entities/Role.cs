using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
