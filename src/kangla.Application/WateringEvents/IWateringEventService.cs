using kangla.Application.Shared;

namespace kangla.Application.WateringEvents
{
    public interface IWateringEventService
    {
        Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForPlantAsync(int plantId, string userId, int pageNumber, int pageSize);
        Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEventDto);
        Task<bool> DeleteWateringEventAsync(int wateringEventId);
    }
}
