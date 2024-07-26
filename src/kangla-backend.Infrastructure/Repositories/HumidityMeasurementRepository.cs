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

    public async Task<IEnumerable<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId)
    {
        return await _context.HumidityMeasurements
                             .Where(h => h.WateringDeviceId == deviceId)
                             .ToListAsync();
    }

    public async Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement)
    {
        _context.HumidityMeasurements.Add(humidityMeasurement);
        await _context.SaveChangesAsync();
    }
}
