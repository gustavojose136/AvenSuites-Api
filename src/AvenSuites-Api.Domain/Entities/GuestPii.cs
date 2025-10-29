using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class GuestPii
{
    [Key]
    [Required]
    public Guid GuestId { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(320)]
    public string? Email { get; set; }
    
    [MaxLength(64)]
    public string? EmailSha256 { get; set; }
    
    [MaxLength(20)]
    public string? PhoneE164 { get; set; }
    
    [MaxLength(64)]
    public string? PhoneSha256 { get; set; }
    
    [MaxLength(30)]
    public string? DocumentType { get; set; }
    
    [MaxLength(32)]
    public string? DocumentPlain { get; set; }
    
    [MaxLength(64)]
    public string? DocumentSha256 { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [MaxLength(160)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(160)]
    public string? AddressLine2 { get; set; }
    
    [MaxLength(120)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Neighborhood { get; set; }
    
    [MaxLength(60)]
    public string? State { get; set; }
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [MaxLength(2)]
    public string CountryCode { get; set; } = "BR";
    
    public byte[]? DocumentCipher { get; set; }
    public byte[]? DocumentNonce { get; set; }
    public byte[]? DocumentTag { get; set; }
    
    public int DocumentKeyVersion { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Guest Guest { get; set; } = null!;
}

