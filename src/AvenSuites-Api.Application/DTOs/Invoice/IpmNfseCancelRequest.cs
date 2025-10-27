using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Invoice;

public class IpmNfseCancelRequest
{
    [Required]
    public Guid InvoiceId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string NfseNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string SerieNfse { get; set; } = "1";
    
    [MaxLength(500)]
    public string? Observation { get; set; }
}

