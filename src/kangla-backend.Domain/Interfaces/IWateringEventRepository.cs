using Domain.Entities;
using Domain.Model;
public interface IWateringEventRepository
{
    Task<PagedResponse<WateringEvent>> GetWateringEventsByPlantIdAsync(int plantId, string userId, int pageNumber, int pageSize);
    Task AddWateringEventAsync(WateringEvent wateringEvent);
}