using Domain.Model;

public interface IWateringDeviceRepository
{
    Task<IEnumerable<WateringDevice>> GetWateringDevicesAsync();
    Task<WateringDevice?> GetWateringDeviceByIdAsync(int id);
    Task AddWateringDeviceAsync(WateringDevice device);
    Task UpdateWateringDeviceAsync(WateringDevice device);
    Task DeleteWateringDeviceAsync(int id);
    bool WateringDeviceExists(int id);
}
