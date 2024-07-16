using Domain.Model;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class WateringDeviceRepository : IWateringDeviceRepository
{
    private readonly WateringContext _context;

    public WateringDeviceRepository(WateringContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WateringDevice>> GetWateringDevicesAsync()
    {
        return await _context.WateringDevices.ToListAsync();
    }

    public async Task<WateringDevice?> GetWateringDeviceByIdAsync(int id)
    {
        return await _context.WateringDevices.FindAsync(id);
    }

    public async Task AddWateringDeviceAsync(WateringDevice device)
    {
        _context.WateringDevices.Add(device);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWateringDeviceAsync(WateringDevice device)
    {
        _context.Entry(device).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWateringDeviceAsync(int id)
    {
        var device = await _context.WateringDevices.FindAsync(id);
        if (device != null)
        {
            _context.WateringDevices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }

    public bool WateringDeviceExists(int id)
    {
        return _context.WateringDevices.Any(e => e.Id == id);
    }
}
