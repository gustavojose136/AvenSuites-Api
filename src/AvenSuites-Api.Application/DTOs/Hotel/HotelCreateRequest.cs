using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Hotel;

public class HotelCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(150)]
    public string? TradeName { get; set; }
    
    [MaxLength(18)]
    public string? Cnpj { get; set; }
    
    [MaxLength(320)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? PhoneE164 { get; set; }
    
    [MaxLength(64)]
    public string Timezone { get; set; } = "America/Sao_Paulo";
    
    [MaxLength(160)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(160)]
    public string? AddressLine2 { get; set; }
    
    [MaxLength(120)]
    public string? City { get; set; }
    
    [MaxLength(60)]
    public string? State { get; set; }
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [MaxLength(2)]
    public string CountryCode { get; set; } = "BR";
}

