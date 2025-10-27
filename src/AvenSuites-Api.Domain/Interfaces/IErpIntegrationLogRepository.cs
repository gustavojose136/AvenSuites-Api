using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IErpIntegrationLogRepository
{
    Task<ErpIntegrationLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<ErpIntegrationLog>> GetByBookingIdAsync(Guid bookingId);
    Task<IEnumerable<ErpIntegrationLog>> GetByInvoiceIdAsync(Guid invoiceId);
    Task<ErpIntegrationLog> AddAsync(ErpIntegrationLog log);
    Task<ErpIntegrationLog> UpdateAsync(ErpIntegrationLog log);
}

