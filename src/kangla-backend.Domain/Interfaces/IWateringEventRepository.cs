using Domain.Model;
public interface IWateringEventRepository
{
    Task<PagedResponse<WateringEvent>> GetWateringEventsByDeviceIdAsync(int deviceId, int pageNumber, int pageSize);
    Task AddWateringEventAsync(WateringEvent wateringEvent);
}