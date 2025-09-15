using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}
