using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Amenity
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
}

