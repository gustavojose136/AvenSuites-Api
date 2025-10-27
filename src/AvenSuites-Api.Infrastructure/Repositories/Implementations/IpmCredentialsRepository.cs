using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class IpmCredentialsRepository : IIpmCredentialsRepository
{
    private readonly ApplicationDbContext _context;

    public IpmCredentialsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IpmCredentials?> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.IpmCredentials
            .FirstOrDefaultAsync(c => c.HotelId == hotelId && c.Active);
    }

    public async Task<IpmCredentials> AddAsync(IpmCredentials credentials)
    {
        credentials.CreatedAt = DateTime.UtcNow;
        credentials.UpdatedAt = DateTime.UtcNow;
        _context.IpmCredentials.Add(credentials);
        await _context.SaveChangesAsync();
        return credentials;
    }

    public async Task<IpmCredentials> UpdateAsync(IpmCredentials credentials)
    {
        credentials.UpdatedAt = DateTime.UtcNow;
        _context.IpmCredentials.Update(credentials);
        await _context.SaveChangesAsync();
        return credentials;
    }

    public async Task DeleteAsync(Guid id)
    {
        var credentials = await _context.IpmCredentials.FindAsync(id);
        if (credentials != null)
        {
            credentials.Active = false;
            credentials.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

