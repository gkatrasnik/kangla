using kangla.Domain.Entities;

namespace kangla.Domain.Interfaces
{
    public interface IWateringCommandRepository
    {
        Task<WateringCommand?> GetActiveForDeviceAsync(int deviceId, DateTime nowUtc);
        Task<WateringCommand?> GetByIdForDeviceAsync(int commandId, int deviceId);
        Task<WateringCommand?> GetByIdForUserAsync(int commandId, int deviceId, string userId);
        Task<(WateringCommand Command, bool Created)> CreateOrGetActiveAsync(WateringCommand command, DateTime nowUtc);
        Task UpdateAsync(WateringCommand command);
        Task CompleteAsync(WateringCommand command, WateringEvent wateringEvent);
    }
}
