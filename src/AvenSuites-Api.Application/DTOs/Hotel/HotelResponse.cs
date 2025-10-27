namespace AvenSuitesApi.Application.DTOs.Hotel;

public class HotelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? PhoneE164 { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

