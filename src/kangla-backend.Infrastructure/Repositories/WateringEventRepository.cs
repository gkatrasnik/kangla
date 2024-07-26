using Domain.Model;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WateringEventRepository : IWateringEventRepository
{
    private readonly WateringContext _context;

    public WateringEventRepository(WateringContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WateringEvent>> GetWateringEventsByDeviceIdAsync(int deviceId)
    {
        return await _context.WateringEvents
                             .Where(e => e.WateringDeviceId == deviceId)
                             .ToListAsync();
    }

    public async Task AddWateringEventAsync(WateringEvent wateringEvent)
    {
        _context.WateringEvents.Add(wateringEvent);
        await _context.SaveChangesAsync();
    }
}
