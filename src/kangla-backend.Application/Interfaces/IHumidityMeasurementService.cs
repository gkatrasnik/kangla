using Application.DTO;

namespace Application.Interfaces
{
    public interface IHumidityMeasurementService
    {
        Task<IEnumerable<HumidityMeasurementResponseDto>> GetHumidityMeasurementsForDeviceAsync(int deviceId);
        Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurement);
    }
}
