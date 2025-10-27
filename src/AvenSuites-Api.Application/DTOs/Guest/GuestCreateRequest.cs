using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Guest;

public class GuestCreateRequest
{
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(320)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? PhoneE164 { get; set; }
    
    [MaxLength(30)]
    public string? DocumentType { get; set; }
    
    [MaxLength(32)]
    public string? DocumentPlain { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
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
    
    public bool MarketingConsent { get; set; }
}

