namespace AvenSuitesApi.Application.DTOs.Invoice;

public class IpmNfseCreateResponse
{
    public bool Success { get; set; }
    public string? NfseNumber { get; set; }
    public string? SerieNfse { get; set; }
    public string? VerificationCode { get; set; }
    public string? XmlContent { get; set; }
    public string? PdfContent { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RawResponse { get; set; }
}

