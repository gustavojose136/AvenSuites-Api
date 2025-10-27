using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> GetByIdWithItemsAsync(Guid id);
    Task<Invoice?> GetByBookingIdAsync(Guid bookingId);
    Task<IEnumerable<Invoice>> GetByHotelIdAsync(Guid hotelId);
    Task<IEnumerable<Invoice>> GetByStatusAsync(string status);
    Task<Invoice> AddAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsNfseNumberUniqueAsync(Guid hotelId, string nfseSeries, string nfseNumber);
}

