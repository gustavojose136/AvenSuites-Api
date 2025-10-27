using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel?> GetByIdAsync(Guid id)
    {
        return await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hotel?> GetByCnpjAsync(string cnpj)
    {
        return await _context.Hotels.FirstOrDefaultAsync(h => h.Cnpj == cnpj);
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _context.Hotels.Where(h => h.Status == "ACTIVE").ToListAsync();
    }

    public async Task<Hotel> AddAsync(Hotel hotel)
    {
        hotel.CreatedAt = DateTime.UtcNow;
        hotel.UpdatedAt = DateTime.UtcNow;
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return hotel;
    }

    public async Task<Hotel> UpdateAsync(Hotel hotel)
    {
        hotel.UpdatedAt = DateTime.UtcNow;
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();
        return hotel;
    }

    public async Task DeleteAsync(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel != null)
        {
            hotel.Status = "INACTIVE";
            hotel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Hotels.AnyAsync(h => h.Id == id && h.Status == "ACTIVE");
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj)
    {
        return await _context.Hotels.AnyAsync(h => h.Cnpj == cnpj);
    }
}

