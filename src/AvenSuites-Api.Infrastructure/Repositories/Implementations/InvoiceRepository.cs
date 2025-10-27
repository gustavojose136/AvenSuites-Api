using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetByIdWithItemsAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetByBookingIdAsync(Guid bookingId)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.BookingId == bookingId);
    }

    public async Task<IEnumerable<Invoice>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Invoices
            .Where(i => i.HotelId == hotelId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status)
    {
        return await _context.Invoices
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Invoice> AddAsync(Invoice invoice)
    {
        invoice.CreatedAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        invoice.UpdatedAt = DateTime.UtcNow;
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Invoices.AnyAsync(i => i.Id == id);
    }

    public async Task<bool> IsNfseNumberUniqueAsync(Guid hotelId, string nfseSeries, string nfseNumber)
    {
        return !await _context.Invoices
            .AnyAsync(i => i.HotelId == hotelId && i.NfseSeries == nfseSeries && i.NfseNumber == nfseNumber);
    }
}

