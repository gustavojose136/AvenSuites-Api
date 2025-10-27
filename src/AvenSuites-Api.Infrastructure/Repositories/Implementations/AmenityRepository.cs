using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class AmenityRepository : IAmenityRepository
{
    private readonly ApplicationDbContext _context;

    public AmenityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Amenity?> GetByIdAsync(Guid id)
    {
        return await _context.Amenities.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Amenity?> GetByCodeAsync(string code)
    {
        return await _context.Amenities.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<IEnumerable<Amenity>> GetAllAsync()
    {
        return await _context.Amenities.ToListAsync();
    }

    public async Task<Amenity> AddAsync(Amenity amenity)
    {
        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();
        return amenity;
    }

    public async Task<Amenity> UpdateAsync(Amenity amenity)
    {
        _context.Amenities.Update(amenity);
        await _context.SaveChangesAsync();
        return amenity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity != null)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Amenities.AnyAsync(a => a.Code == code);
    }
}

