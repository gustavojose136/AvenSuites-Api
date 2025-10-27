using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class RatePlanPrice
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid RatePlanId { get; set; }
    
    [Required]
    public DateTime PriceDate { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal PriceAmount { get; set; }
    
    public int? MinStay { get; set; } = 1;
    public int? MaxStay { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual RatePlan RatePlan { get; set; } = null!;
}

