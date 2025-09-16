using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.UserRoles)
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<Role> AddAsync(Role role)
    {
        role.CreatedAt = DateTime.UtcNow;
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Roles.AnyAsync(r => r.Id == id && r.IsActive);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Roles.AnyAsync(r => r.Name == name && r.IsActive);
    }
}
