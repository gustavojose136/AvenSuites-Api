using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly ApplicationDbContext _context;

    public RoomTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoomType?> GetByIdAsync(Guid id)
    {
        return await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RoomType?> GetByIdWithAmenitiesAsync(Guid id)
    {
        return await _context.RoomTypes
            .Include(rt => rt.Amenities)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RoomType?> GetByIdWithOccupancyPricesAsync(Guid id)
    {
        return await _context.RoomTypes
            .Include(rt => rt.OccupancyPrices)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<IEnumerable<RoomType>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.RoomTypes
            .Where(rt => rt.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RoomType>> GetActiveByHotelIdAsync(Guid hotelId)
    {
        return await _context.RoomTypes
            .Where(rt => rt.HotelId == hotelId && rt.Active)
            .ToListAsync();
    }

    public async Task<RoomType> AddAsync(RoomType roomType)
    {
        roomType.CreatedAt = DateTime.UtcNow;
        roomType.UpdatedAt = DateTime.UtcNow;
        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync();
        return roomType;
    }

    public async Task<RoomType> UpdateAsync(RoomType roomType)
    {
        roomType.UpdatedAt = DateTime.UtcNow;
        _context.RoomTypes.Update(roomType);
        await _context.SaveChangesAsync();
        return roomType;
    }

    public async Task DeleteAsync(Guid id)
    {
        var roomType = await _context.RoomTypes.FindAsync(id);
        if (roomType != null)
        {
            roomType.Active = false;
            roomType.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.RoomTypes.AnyAsync(rt => rt.Id == id);
    }

    public async Task<bool> IsCodeUniqueAsync(Guid hotelId, string code, Guid? excludeRoomTypeId = null)
    {
        var query = _context.RoomTypes.Where(rt => rt.HotelId == hotelId && rt.Code == code);
        
        if (excludeRoomTypeId.HasValue)
            query = query.Where(rt => rt.Id != excludeRoomTypeId.Value);

        return !await query.AnyAsync();
    }
}

