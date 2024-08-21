using Application.DTO;

namespace Application.Interfaces
{
    public interface IWateringEventService
    {
        Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForPlantAsync(int plantId, string userId, int pageNumber, int pageSize);
        Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEventDto);
    }
}
