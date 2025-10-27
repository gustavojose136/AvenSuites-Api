using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class MaintenanceBlockRepository : IMaintenanceBlockRepository
{
    private readonly ApplicationDbContext _context;

    public MaintenanceBlockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceBlock?> GetByIdAsync(Guid id)
    {
        return await _context.MaintenanceBlocks
            .Include(mb => mb.Room)
            .FirstOrDefaultAsync(mb => mb.Id == id);
    }

    public async Task<IEnumerable<MaintenanceBlock>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.MaintenanceBlocks
            .Where(mb => mb.RoomId == roomId)
            .OrderByDescending(mb => mb.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceBlock>> GetActiveBlocksByDateRangeAsync(Guid roomId, DateTime startDate, DateTime endDate)
    {
        return await _context.MaintenanceBlocks
            .Where(mb => mb.RoomId == roomId
                && mb.Status == "ACTIVE"
                && mb.StartDate <= endDate
                && mb.EndDate >= startDate)
            .OrderBy(mb => mb.StartDate)
            .ToListAsync();
    }

    public async Task<MaintenanceBlock> AddAsync(MaintenanceBlock maintenanceBlock)
    {
        maintenanceBlock.CreatedAt = DateTime.UtcNow;
        maintenanceBlock.UpdatedAt = DateTime.UtcNow;
        _context.MaintenanceBlocks.Add(maintenanceBlock);
        await _context.SaveChangesAsync();
        return maintenanceBlock;
    }

    public async Task<MaintenanceBlock> UpdateAsync(MaintenanceBlock maintenanceBlock)
    {
        maintenanceBlock.UpdatedAt = DateTime.UtcNow;
        _context.MaintenanceBlocks.Update(maintenanceBlock);
        await _context.SaveChangesAsync();
        return maintenanceBlock;
    }

    public async Task DeleteAsync(Guid id)
    {
        var maintenanceBlock = await _context.MaintenanceBlocks.FindAsync(id);
        if (maintenanceBlock != null)
        {
            maintenanceBlock.Status = "INACTIVE";
            maintenanceBlock.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

