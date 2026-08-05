using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Repositories
{
    public class WateringEventRepository : IWateringEventRepository
    {
        private readonly PlantsContext _context;

        public WateringEventRepository(PlantsContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<WateringEvent>> GetWateringEventsByPlantIdAsync(int plantId, string userId, int pageNumber, int pageSize)
        {
            var totalRecords = await _context.WateringEvents.AsNoTracking()
                .Where(e => e.PlantId == plantId && e.Plant.UserId == userId)
                .CountAsync();

            var wateringEvents = await _context.WateringEvents.AsNoTracking()
                                 .Where(e => e.PlantId == plantId && e.Plant.UserId == userId)
                                 .OrderByDescending(x => x.CreatedAt)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            return new PagedResponse<WateringEvent>(wateringEvents, pageNumber, pageSize, totalRecords);
        }

        public async Task AddWateringEventAsync(WateringEvent wateringEvent)
        {
            _context.WateringEvents.Add(wateringEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> DeleteWateringEventAsync(int wateringEventId, string userId)
        {
            var wateringEvent = await _context.WateringEvents
                .Include(e => e.Plant)
                .FirstOrDefaultAsync(e => e.Id == wateringEventId && e.Plant.UserId == userId);
            if (wateringEvent != null)
            {
                var plantId = wateringEvent.PlantId;
                _context.WateringEvents.Remove(wateringEvent);
                await _context.SaveChangesAsync();
                return plantId;
            }

            return null;
        }

        public async Task<DateTime?> GetLastWateringEventDateAsync(int plantId)
        {
            return await _context.WateringEvents
            .Where(w => w.PlantId == plantId)
            .OrderByDescending(w => w.Start)
            .Select(w => (DateTime?)w.Start) // Cast to nullable DateTime
            .FirstOrDefaultAsync();
        }
    }
}
