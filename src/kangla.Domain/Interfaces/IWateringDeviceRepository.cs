using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IWateringDeviceRepository
    {
        Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize);
        Task<WateringDevice?> GetWateringDeviceByIdAsync(int deviceId, string userId);
        Task<WateringDevice?> GetWateringDeviceByPlantIdAsync(int plantId, string userId);
        Task<WateringDevice?> GetWateringDeviceByCredentialHashAsync(string credentialHash);
        Task<WateringDevice?> GetLegacyWateringDeviceByTokenAsync(string deviceToken);
        Task AddWateringDeviceAsync(WateringDevice device);
        Task UpdateWateringDeviceAsync(WateringDevice device, string userId);
        Task<bool> DeleteWateringDeviceAsync(int id, string userId);
        Task<bool> WateringDeviceExistsAsync(int deviceId);
        Task<bool> WateringDeviceExistsForUserAsync(int deviceId, string userId);
        Task<bool> WateringDeviceTokenExistsAsync(string deviceToken);
    }
}
