using Domain.Model;

public interface IHumidityMeasurementRepository
{
    Task<PagedResponse<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId, int pageNumber, int pageSize);
    Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement);
}
