using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IWateringCommandRepository
    {
        Task<WateringCommand?> GetActiveForDeviceAsync(int deviceId, DateTime nowUtc);
        Task<IReadOnlyCollection<WateringCommand>> GetActiveForDevicesAsync(IReadOnlyCollection<int> deviceIds, DateTime nowUtc);
        Task<WateringCommand?> GetByIdForDeviceAsync(int commandId, int deviceId);
        Task<WateringCommand?> GetByIdForUserAsync(int commandId, int deviceId, string userId);
        Task<PagedResponse<WateringCommand>> GetForDeviceForUserAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task<(WateringCommand Command, bool Created)> CreateOrGetActiveAsync(WateringCommand command, DateTime nowUtc);
        Task UpdateAsync(WateringCommand command);
        Task<bool> TrySetStatusAsync(int commandId, WateringCommandStatus expectedStatus, WateringCommandStatus newStatus);
        Task<bool> TryAcknowledgeAsync(int commandId, DateTime acknowledgedAtUtc);
        Task<bool> TryFailAsync(WateringCommand command);
        Task<bool> TryCompleteAsync(WateringCommand command, WateringEvent wateringEvent);
    }
}
