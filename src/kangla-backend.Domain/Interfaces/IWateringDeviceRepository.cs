using Domain.Model;

public interface IWateringDeviceRepository
{
    public Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(int pageNumber, int pageSize);
    Task<WateringDevice?> GetWateringDeviceByIdAsync(int id);
    Task AddWateringDeviceAsync(WateringDevice device);
    Task UpdateWateringDeviceAsync(WateringDevice device);
    Task DeleteWateringDeviceAsync(int id);
    bool WateringDeviceExists(int id);
}
