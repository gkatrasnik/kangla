using kangla.Application.DTO;

namespace kangla.Application.Interfaces
{
    public interface IWateringEventService
    {
        Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForPlantAsync(int plantId, string userId, int pageNumber, int pageSize);
        Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEventDto);
        Task<bool> DeleteWateringEventAsync(int wateringEventId);
    }
}
