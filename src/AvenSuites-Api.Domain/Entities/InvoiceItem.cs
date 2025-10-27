using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid InvoiceId { get; set; }
    
    [Required]
    [MaxLength(240)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 9999999999.99)]
    public decimal Quantity { get; set; } = 1.00m;
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal UnitPrice { get; set; }
    
    [MaxLength(30)]
    public string? TaxCode { get; set; }
    
    public decimal? TaxRate { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal Total { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}

