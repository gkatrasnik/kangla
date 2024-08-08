using Domain.Model;

public interface IHumidityMeasurementRepository
{
    Task<PagedResponse<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId, string userId, int pageNumber, int pageSize);
    Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement);
}
