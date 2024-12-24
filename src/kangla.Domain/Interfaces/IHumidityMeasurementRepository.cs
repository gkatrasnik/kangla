using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IHumidityMeasurementRepository
    {
        Task<PagedResponse<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement);
    }
}