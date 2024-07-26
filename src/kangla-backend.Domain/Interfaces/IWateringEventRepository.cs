using Domain.Model;
public interface IWateringEventRepository
{
    Task<IEnumerable<WateringEvent>> GetWateringEventsByDeviceIdAsync(int deviceId);
    Task AddWateringEventAsync(WateringEvent wateringEvent);
}