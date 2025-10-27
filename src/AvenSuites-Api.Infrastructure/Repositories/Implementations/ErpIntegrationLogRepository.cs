using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class ErpIntegrationLogRepository : IErpIntegrationLogRepository
{
    private readonly ApplicationDbContext _context;

    public ErpIntegrationLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErpIntegrationLog?> GetByIdAsync(Guid id)
    {
        return await _context.ErpIntegrationLogs.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<ErpIntegrationLog>> GetByBookingIdAsync(Guid bookingId)
    {
        return await _context.ErpIntegrationLogs
            .Where(l => l.BookingId == bookingId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ErpIntegrationLog>> GetByInvoiceIdAsync(Guid invoiceId)
    {
        return await _context.ErpIntegrationLogs
            .Where(l => l.InvoiceId == invoiceId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<ErpIntegrationLog> AddAsync(ErpIntegrationLog log)
    {
        log.CreatedAt = DateTime.UtcNow;
        _context.ErpIntegrationLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<ErpIntegrationLog> UpdateAsync(ErpIntegrationLog log)
    {
        _context.ErpIntegrationLogs.Update(log);
        await _context.SaveChangesAsync();
        return log;
    }
}

