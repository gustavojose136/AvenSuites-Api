using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> AddAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
