using Application.DTO;


namespace Application.Interfaces
{
    public interface IWateringEventService
    {
        Task<IEnumerable<WateringEventResponseDto>> GetWateringEventsForDeviceAsync(int deviceId);
        Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent);
    }
}
