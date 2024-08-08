using Application.DTO;

namespace Application.Interfaces
{
    public interface IWateringDeviceService
    {
        Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize);
        Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int deviceId, string userId);
        Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice, string userId);
        Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDevice);
        Task<bool> DeleteWateringDeviceAsync(int deviceId, string userId);
    }
}
