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

    public async Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
    {
        var totalRecords = await _context.WateringDevices.AsNoTracking().CountAsync();
        var wateringDevices = await _context.WateringDevices.AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<WateringDevice>(wateringDevices, pageNumber, pageSize, totalRecords);
    }

    public async Task<WateringDevice?> GetWateringDeviceByIdAsync(int deviceId, string userId)
    {
        return await _context.WateringDevices.AsNoTracking()
            .Where(d => d.Id == deviceId && d.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task AddWateringDeviceAsync(WateringDevice device)
    {
        _context.WateringDevices.Add(device);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWateringDeviceAsync(WateringDevice device, string userId)
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        var existingDevice = await _context.WateringDevices
            .FirstOrDefaultAsync(d => d.Id == device.Id && d.UserId == userId);

        if (existingDevice == null)
        {
            throw new InvalidOperationException($"WateringDevice with Id {device.Id} does not exist for current user.");
        }

        _context.Entry(existingDevice).CurrentValues.SetValues(device);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteWateringDeviceAsync(int deviceId)
    {
        var device = await _context.WateringDevices.FindAsync(deviceId);
        if (device != null)
        {
            _context.WateringDevices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> WateringDeviceExistsAsync(int deviceId)
    {
        return await _context.WateringDevices.AnyAsync(e => e.Id == deviceId);
    }

    public async Task<bool> WateringDeviceExistsForUserAsync(int deviceId, string userId)
    {
        return await _context.WateringDevices.AnyAsync(e => e.Id == deviceId && e.UserId == userId);
    }

    public async Task<bool> WateringDeviceTokenExistsAsync(string deviceToken)
    {
        return await _context.WateringDevices
            .AnyAsync(d => d.DeviceToken == deviceToken);
    }
}
