namespace AvenSuitesApi.Application.DTOs.Invoice;

public class InvoiceResponse
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public string? NfseNumber { get; set; }
    public string? NfseSeries { get; set; }
    public string? RpsNumber { get; set; }
    public string? VerificationCode { get; set; }
    public string? ErpProvider { get; set; }
    public string? ErpProtocol { get; set; }
    public decimal TotalServices { get; set; }
    public decimal TotalTaxes { get; set; }
    public string? XmlS3Key { get; set; }
    public string? PdfS3Key { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<InvoiceItemResponse> Items { get; set; } = new();
}

public class InvoiceItemResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? TaxCode { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal Total { get; set; }
}

