using kangla.Application.Shared;

namespace kangla.Application.HumidityMeasurements
{
    public interface IHumidityMeasurementService
    {
        Task<PagedResponseDto<HumidityMeasurementResponseDto>> GetHumidityMeasurementsForDeviceAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurementDto);
    }
}
