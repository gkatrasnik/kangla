using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IWateringDeviceRepository
    {
        Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize);
        Task<WateringDevice?> GetWateringDeviceByIdAsync(int deviceId, string userId);
        Task<WateringDevice?> GetWateringDeviceByPlantIdAsync(int plantId, string userId);
        Task<WateringDevice?> GetWateringDeviceByAccessKeyHashAsync(string accessKeyHash);
        Task<WateringDevice?> GetUnclaimedWateringDeviceByAccessKeyHashAsync(string accessKeyHash);
        Task ClaimWateringDeviceAsync(WateringDevice device, string userId);
        Task UpdateWateringDeviceAsync(WateringDevice device, string userId);
        Task<bool> DetachWateringDeviceAsync(int id, string userId);
        Task<bool> DeleteWateringDeviceAsync(int id, string userId);
        Task<bool> WateringDeviceExistsAsync(int deviceId);
        Task<bool> WateringDeviceExistsForUserAsync(int deviceId, string userId);
    }
}
