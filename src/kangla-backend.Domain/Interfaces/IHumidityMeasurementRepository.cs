using Domain.Model;

public interface IHumidityMeasurementRepository
{
    Task<IEnumerable<HumidityMeasurement>> GetHumidityMeasurementsByDeviceIdAsync(int deviceId);
    Task AddHumidityMeasurementAsync(HumidityMeasurement humidityMeasurement);
}
