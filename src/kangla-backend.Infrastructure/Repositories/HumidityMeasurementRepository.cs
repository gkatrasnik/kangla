using Domain.Model;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class HumidityMeasurementRepository : IHumidityMeasurementRepository
{
    private readonly WateringContext _context;

    public HumidityMeasurementRepository(WateringContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize)
    {
        var totalRecords = await _context.HumidityMeasurements.AsNoTracking()
            .Where(h => h.WateringDeviceId == deviceId && h.WateringDevice.UserId == userId)
            .CountAsync();

        var humidityMeasurements = await _context.HumidityMeasurements.AsNoTracking()
                             .Where(h => h.WateringDeviceId == deviceId && h.WateringDevice.UserId == userId)
                             .OrderBy(x => x.DateTime)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();

        return new PagedResponse<HumidityMeasurement>(humidityMeasurements, pageNumber, pageSize, totalRecords);
    }

    public async Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement)
    {
        _context.HumidityMeasurements.Add(humidityMeasurement);
        await _context.SaveChangesAsync();
    }
}
