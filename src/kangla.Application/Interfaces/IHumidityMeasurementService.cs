using kangla.Application.DTO;

namespace kangla.Application.Interfaces
{
    public interface IHumidityMeasurementService
    {
        Task<PagedResponseDto<HumidityMeasurementResponseDto>> GetHumidityMeasurementsForDeviceAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurementDto);
    }
}
