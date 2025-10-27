namespace AvenSuitesApi.Application.DTOs.Guest;

public class GuestResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneE164 { get; set; }
    public bool MarketingConsent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

