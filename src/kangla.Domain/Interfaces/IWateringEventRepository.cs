using Domain.Entities;
using Domain.Model;
public interface IWateringEventRepository
{
    Task<PagedResponse<WateringEvent>> GetWateringEventsByPlantIdAsync(int plantId, string userId, int pageNumber, int pageSize);
    Task AddWateringEventAsync(WateringEvent wateringEvent);
    Task DeleteWateringEventAsync(int wateringEventId);
    Task<bool> WateringEventExistsAsync(int wateringEventId);
    Task<DateTime?> GetLastWateringEventDateAsync(int plantId);
}