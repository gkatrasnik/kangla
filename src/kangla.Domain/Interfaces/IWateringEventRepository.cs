using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IWateringEventRepository
    {
        Task<PagedResponse<WateringEvent>> GetWateringEventsByPlantIdAsync(int plantId, string userId, int pageNumber, int pageSize);
        Task AddWateringEventAsync(WateringEvent wateringEvent);
        Task<bool> DeleteWateringEventAsync(int wateringEventId, string userId);
        Task<DateTime?> GetLastWateringEventDateAsync(int plantId);
    }
}
