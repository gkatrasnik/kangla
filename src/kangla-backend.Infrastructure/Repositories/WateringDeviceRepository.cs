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

    public async Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.WateringDevices.AsNoTracking().CountAsync();

        var wateringDevices = await _context.WateringDevices.AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<WateringDevice>(wateringDevices, pageNumber, pageSize, totalRecords);
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
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        var existingDevice = await _context.WateringDevices
            .FirstOrDefaultAsync(d => d.Id == device.Id);

        if (existingDevice == null)
        {
            throw new InvalidOperationException($"WateringDevice with Id {device.Id} does not exist.");
        }

        existingDevice.Name = device.Name;
        existingDevice.Description = device.Description;
        existingDevice.Location = device.Location;
        existingDevice.Notes = device.Notes;
        existingDevice.Active = device.Active;
        existingDevice.Deleted = device.Deleted;
        existingDevice.WaterNow = device.WaterNow;
        existingDevice.MinimumSoilHumidity = device.MinimumSoilHumidity;
        existingDevice.WateringIntervalSetting = device.WateringIntervalSetting;
        existingDevice.WateringDurationSetting = device.WateringDurationSetting;
        existingDevice.DeviceToken = device.DeviceToken;

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
