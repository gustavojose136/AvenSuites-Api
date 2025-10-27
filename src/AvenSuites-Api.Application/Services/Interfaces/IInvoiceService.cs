using AvenSuitesApi.Application.DTOs.Invoice;

namespace AvenSuitesApi.Application.Services.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse?> GenerateInvoiceAsync(Guid bookingId);
    Task<InvoiceResponse?> GenerateInvoiceFromBookingAsync(Guid bookingId);
    Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id);
    Task<InvoiceResponse?> GetInvoiceByBookingIdAsync(Guid bookingId);
    Task<IEnumerable<InvoiceResponse>> GetInvoicesByHotelAsync(Guid hotelId);
    Task<bool> EmitInvoiceToErpAsync(Guid invoiceId);
    Task<InvoiceResponse?> CancelInvoiceAsync(Guid invoiceId);
}

