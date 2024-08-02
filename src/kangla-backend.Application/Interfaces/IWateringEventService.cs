using Application.DTO;


namespace Application.Interfaces
{
    public interface IWateringEventService
    {
        Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForDeviceAsync(int deviceId, int pageNumber, int pageSize);
        Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent);
    }
}
