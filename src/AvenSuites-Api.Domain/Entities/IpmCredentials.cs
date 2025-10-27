using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class IpmCredentials
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? CpfCnpj { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string CityCode { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? SerieNfse { get; set; } = "1";
    
    public bool Active { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Hotel Hotel { get; set; } = null!;
}

