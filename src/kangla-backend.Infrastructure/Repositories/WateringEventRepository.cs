using Domain.Model;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class WateringEventRepository : IWateringEventRepository
{
    private readonly WateringContext _context;

    public WateringEventRepository(WateringContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<WateringEvent>> GetWateringEventsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize)
    {
        var totalRecords = await _context.WateringEvents.AsNoTracking()
            .Where(e => e.WateringDeviceId == deviceId && e.WateringDevice.UserId == userId)
            .CountAsync();

        var wateringEvents = await _context.WateringEvents.AsNoTracking()
                             .Where(e => e.WateringDeviceId == deviceId && e.WateringDevice.UserId == userId)
                             .OrderBy(x => x.Start)
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
}
