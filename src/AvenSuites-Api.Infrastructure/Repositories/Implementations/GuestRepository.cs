using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class GuestRepository : IGuestRepository
{
    private readonly ApplicationDbContext _context;

    public GuestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guest?> GetByIdAsync(Guid id)
    {
        return await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Guest?> GetByUserId(Guid userId)
    {
        return await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);
    }

    public async Task<Guest?> GetByIdWithPiiAsync(Guid id)
    {
        return await _context.Guests
            .Include(g => g.GuestPii)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Guest>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Guests
            .Include(g => g.GuestPii)
            .Where(g => g.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<Guest> AddAsync(Guest guest)
    {
        guest.CreatedAt = DateTime.UtcNow;
        guest.UpdatedAt = DateTime.UtcNow;
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();
        return guest;
    }

    public async Task<Guest> UpdateAsync(Guest guest)
    {
        guest.UpdatedAt = DateTime.UtcNow;
        _context.Guests.Update(guest);
        await _context.SaveChangesAsync();
        return guest;
    }

    public async Task DeleteAsync(Guid id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest != null)
        {
            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Guests.AnyAsync(g => g.Id == id);
    }
}

