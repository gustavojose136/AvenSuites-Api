namespace AvenSuitesApi.Application.DTOs.Guest;

public class GuestProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Neighborhood { get; set; }
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool MarketingConsent { get; set; }
    public DateTime CreatedAt { get; set; }
}

