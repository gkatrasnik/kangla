using Application.DTO;

namespace Application.Interfaces
{
    public interface IWateringDeviceService
    {
        Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(int pageNumber, int pageSize);
        Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int id);
        Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice);
        Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int id, WateringDeviceUpdateRequestDto wateringDevice);
        Task<bool> DeleteWateringDeviceAsync(int id);
        bool WateringDeviceExists(int id);
    }
}
