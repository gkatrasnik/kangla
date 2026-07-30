namespace kangla.Application.WateringCommands
{
    public interface IWateringCommandService
    {
        Task<(WateringCommandResponseDto Command, bool Created)> CreateForUserAsync(int deviceId, string userId);
        Task<WateringCommandResponseDto> GetForUserAsync(int deviceId, int commandId, string userId);
        Task CancelForUserAsync(int deviceId, int commandId, string userId);
        Task<DeviceCheckInResponseDto> CheckInAsync(DeviceCheckInRequestDto request, string deviceCredential);
        Task<WateringCommandResponseDto> AcknowledgeAsync(int commandId, string deviceCredential);
        Task<WateringCommandResponseDto> ReportResultAsync(int commandId, DeviceWateringCommandResultRequestDto request, string deviceCredential);
    }
}
