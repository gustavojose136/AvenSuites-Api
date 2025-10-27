using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class RatePlanRepository : IRatePlanRepository
{
    private readonly ApplicationDbContext _context;

    public RatePlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RatePlan?> GetByIdAsync(Guid id)
    {
        return await _context.RatePlans.FirstOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<RatePlan?> GetByIdWithPricesAsync(Guid id)
    {
        return await _context.RatePlans
            .Include(rp => rp.Prices)
            .FirstOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<IEnumerable<RatePlan>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.RatePlans
            .Where(rp => rp.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RatePlan>> GetByRoomTypeIdAsync(Guid roomTypeId)
    {
        return await _context.RatePlans
            .Where(rp => rp.RoomTypeId == roomTypeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RatePlan>> GetActiveByHotelIdAsync(Guid hotelId)
    {
        return await _context.RatePlans
            .Where(rp => rp.HotelId == hotelId && rp.Active)
            .ToListAsync();
    }

    public async Task<RatePlan> AddAsync(RatePlan ratePlan)
    {
        ratePlan.CreatedAt = DateTime.UtcNow;
        ratePlan.UpdatedAt = DateTime.UtcNow;
        _context.RatePlans.Add(ratePlan);
        await _context.SaveChangesAsync();
        return ratePlan;
    }

    public async Task<RatePlan> UpdateAsync(RatePlan ratePlan)
    {
        ratePlan.UpdatedAt = DateTime.UtcNow;
        _context.RatePlans.Update(ratePlan);
        await _context.SaveChangesAsync();
        return ratePlan;
    }

    public async Task DeleteAsync(Guid id)
    {
        var ratePlan = await _context.RatePlans.FindAsync(id);
        if (ratePlan != null)
        {
            ratePlan.Active = false;
            ratePlan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.RatePlans.AnyAsync(rp => rp.Id == id);
    }
}

