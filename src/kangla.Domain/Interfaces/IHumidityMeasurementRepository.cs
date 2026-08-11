using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IHumidityMeasurementRepository
    {
        Task<PagedResponse<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task<IReadOnlyDictionary<int, HumidityMeasurement>> GetLatestHumidityMeasurementsByDeviceIdsAsync(IReadOnlyCollection<int> deviceIds);
        Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement);
    }
}
