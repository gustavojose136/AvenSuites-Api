using AvenSuitesApi.Application.DTOs.Invoice;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IIpmNfseService
{
    Task<IpmNfseCreateResponse> GenerateInvoiceAsync(Guid hotelId, IpmNfseCreateRequest request);
    Task<IpmNfseCreateResponse> CancelInvoiceAsync(Guid hotelId, IpmNfseCancelRequest request);
    Task<IpmNfseCreateResponse> GetInvoiceByVerificationCodeAsync(Guid hotelId, string verificationCode);
    Task<IpmNfseCreateResponse> GetInvoiceByNumberAsync(Guid hotelId, string nfseNumber, string serie);
    
    /// <summary>
    /// Cria NF-e de forma simplificada - busca hotel pelo roomId e preenche dados automaticamente
    /// </summary>
    Task<IpmNfseCreateResponse> GenerateSimpleInvoiceAsync(Guid roomId, SimpleInvoiceCreateRequest request);
}

