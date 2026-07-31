using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Repositories
{
    public class WateringDeviceRepository : IWateringDeviceRepository
    {
        private readonly PlantsContext _context;

        public WateringDeviceRepository(PlantsContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<WateringDevice>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
        {
            var totalRecords = await _context.WateringDevices.AsNoTracking()
                .Where(w => w.UserId == userId && !w.IsDeleted)
                .CountAsync();
            var wateringDevices = await _context.WateringDevices.AsNoTracking()
                .Where(w => w.UserId == userId && !w.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<WateringDevice>(wateringDevices, pageNumber, pageSize, totalRecords);
        }

        public async Task<WateringDevice?> GetWateringDeviceByIdAsync(int deviceId, string userId)
        {
            return await _context.WateringDevices.AsNoTracking()
                .Where(d => d.Id == deviceId && d.UserId == userId && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<WateringDevice?> GetWateringDeviceByPlantIdAsync(int plantId, string userId)
        {
            return await _context.WateringDevices.AsNoTracking()
                .Where(d => d.PlantId == plantId && d.UserId == userId && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<WateringDevice?> GetWateringDeviceByAccessKeyHashAsync(string accessKeyHash)
        {
            return await _context.WateringDevices.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeviceAccessKeyHash == accessKeyHash && !d.IsDeleted);
        }

        public async Task<WateringDevice?> GetUnclaimedWateringDeviceByAccessKeyHashAsync(string accessKeyHash)
        {
            return await _context.WateringDevices.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeviceAccessKeyHash == accessKeyHash && d.UserId == null && d.PlantId == null);
        }

        public async Task ClaimWateringDeviceAsync(WateringDevice device, string userId)
        {
            var existingDevice = await _context.WateringDevices
                .FirstOrDefaultAsync(d => d.Id == device.Id && d.UserId == null && d.PlantId == null)
                ?? throw new InvalidOperationException("The device has already been claimed.");

            _context.Entry(existingDevice).CurrentValues.SetValues(device);
            existingDevice.IsDeleted = false;
            existingDevice.DeletedAtUtc = null;
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

        public async Task<bool> DetachWateringDeviceAsync(int deviceId, string userId)
        {
            var device = await _context.WateringDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);
            if (device != null)
            {
                device.PlantId = null;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteWateringDeviceAsync(int deviceId, string userId)
        {
            var device = await _context.WateringDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);
            if (device is null)
            {
                return false;
            }

            device.UserId = null;
            device.PlantId = null;
            device.IsDeleted = true;
            device.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WateringDeviceExistsAsync(int deviceId)
        {
            return await _context.WateringDevices.AnyAsync(e => e.Id == deviceId);
        }

        public async Task<bool> WateringDeviceExistsForUserAsync(int deviceId, string userId)
        {
            return await _context.WateringDevices.AnyAsync(e => e.Id == deviceId && e.UserId == userId && !e.IsDeleted);
        }

    }
}
