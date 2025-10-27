using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class ApiIdempotencyKey
{
    [Key]
    [Required]
    [MaxLength(80)]
    public string IdempotencyKey { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(80)]
    public string Scope { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(64)]
    public string RequestHash { get; set; } = string.Empty;
    
    public int? ResponseCode { get; set; }
    
    public string? ResponseBody { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime ExpiresAt { get; set; }
}

