using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Repositories
{
    public class HumidityMeasurementRepository : IHumidityMeasurementRepository
    {
        private readonly PlantsContext _context;

        public HumidityMeasurementRepository(PlantsContext context)
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
                                 .OrderByDescending(x => x.DateTime)
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
}