using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BookingId { get; set; }
    
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "PENDING";
    
    public DateTime? IssueDate { get; set; }
    
    [MaxLength(40)]
    public string? NfseNumber { get; set; }
    
    [MaxLength(10)]
    public string? NfseSeries { get; set; }
    
    [MaxLength(40)]
    public string? RpsNumber { get; set; }
    
    [MaxLength(80)]
    public string? VerificationCode { get; set; }
    
    [MaxLength(60)]
    public string? ErpProvider { get; set; }
    
    [MaxLength(120)]
    public string? ErpProtocol { get; set; }
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal TotalServices { get; set; } = 0.00m;
    
    [Required]
    [Range(0, 999999999999.99)]
    public decimal TotalTaxes { get; set; } = 0.00m;
    
    [MaxLength(300)]
    public string? XmlS3Key { get; set; }
    
    [MaxLength(300)]
    public string? PdfS3Key { get; set; }
    
    public string? RawResponseJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

