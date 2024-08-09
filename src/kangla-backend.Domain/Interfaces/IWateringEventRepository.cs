using Domain.Entities;
using Domain.Model;
public interface IWateringEventRepository
{
    Task<PagedResponse<WateringEvent>> GetWateringEventsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize);
    Task AddWateringEventAsync(WateringEvent wateringEvent);
}