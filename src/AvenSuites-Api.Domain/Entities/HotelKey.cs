using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class HotelKey
{
    [Key]
    [Required]
    public Guid HotelId { get; set; }
    
    [Key]
    [Required]
    public int KeyVersion { get; set; }
    
    [Required]
    public byte[] KdfSalt { get; set; } = Array.Empty<byte>();
    
    public DateTime CreatedAt { get; set; }
    
    public bool Active { get; set; } = true;
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
}

