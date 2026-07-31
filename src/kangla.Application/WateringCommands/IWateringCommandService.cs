using kangla.Application.Shared;

namespace kangla.Application.WateringCommands
{
    public interface IWateringCommandService
    {
        Task<(WateringCommandResponseDto Command, bool Created)> CreateForUserAsync(int deviceId, string userId);
        Task<WateringCommandResponseDto> GetForUserAsync(int deviceId, int commandId, string userId);
        Task<PagedResponseDto<WateringCommandResponseDto>> GetForDeviceForUserAsync(int deviceId, string userId, int pageNumber, int pageSize);
        Task CancelForUserAsync(int deviceId, int commandId, string userId);
        Task<DeviceCheckInResponseDto> CheckInAsync(DeviceCheckInRequestDto request, string deviceAccessKey);
        Task<WateringCommandResponseDto> AcknowledgeAsync(int commandId, string deviceAccessKey);
        Task<WateringCommandResponseDto> ReportResultAsync(int commandId, DeviceWateringCommandResultRequestDto request, string deviceAccessKey);
    }
}
