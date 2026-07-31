using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IWateringCommandRepository
    {
        Task<WateringCommand?> GetActiveForDeviceAsync(int deviceId, DateTime nowUtc);
        Task<WateringCommand?> GetByIdForDeviceAsync(int commandId, int deviceId);
        Task<WateringCommand?> GetByIdForUserAsync(int commandId, int deviceId, string userId);
        Task<PagedResponse<WateringCommand>> GetForDeviceForUserAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task<(WateringCommand Command, bool Created)> CreateOrGetActiveAsync(WateringCommand command, DateTime nowUtc);
        Task UpdateAsync(WateringCommand command);
        Task CompleteAsync(WateringCommand command, WateringEvent wateringEvent);
    }
}
