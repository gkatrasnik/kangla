using kangla.Application.Plants.DTO;
using kangla.Application.Shared;

namespace kangla.Application.Plants
{
    public interface IPlantsService
    {
        Task<PagedResponseDto<PlantResponseDto>> GetPlantsAsync(string userId, int pageNumber, int pageSize);
        Task<PlantResponseDto> GetPlantAsync(int plantId, string userId);
        Task<PlantResponseDto> CreatePlantAsync(PlantCreateRequestDto plantDto, string userId);
        Task<PlantResponseDto> UpdatePlantAsync(int plantId, string userId, PlantUpdateRequestDto plantDto);
        Task<bool> DeletePlantAsync(int plantId, string userId);
        Task<PlantRecognizeResponseDto> RecognizePlantAsync(PlantRecognizeRequestDto plantRecognizeDto);
    }
}
