using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IMaintenanceBlockRepository
{
    Task<MaintenanceBlock?> GetByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceBlock>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<MaintenanceBlock>> GetActiveBlocksByDateRangeAsync(Guid roomId, DateTime startDate, DateTime endDate);
    Task<MaintenanceBlock> AddAsync(MaintenanceBlock maintenanceBlock);
    Task<MaintenanceBlock> UpdateAsync(MaintenanceBlock maintenanceBlock);
    Task DeleteAsync(Guid id);
}

