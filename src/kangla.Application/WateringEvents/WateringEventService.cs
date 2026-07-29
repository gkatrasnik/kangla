using AutoMapper;
using kangla.Application.Shared;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;

namespace kangla.Application.WateringEvents
{
    public class WateringEventService : IWateringEventService
    {
        private readonly IWateringEventRepository _wateringEventRepository;
        private readonly IPlantsRepository _plantsRepository;
        private readonly IMapper _mapper;

        public WateringEventService(IWateringEventRepository wateringEventRepository, IPlantsRepository plantsRepository, IMapper mapper)
        {
            _wateringEventRepository = wateringEventRepository;
            _plantsRepository = plantsRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForPlantAsync(int plantId, string userId, int pageNumber, int pageSize)
        {
            var plantExists = await _plantsRepository.PlantExistsForUserAsync(plantId, userId);
            if (!plantExists)
            {
                throw new ArgumentException($"Plant with ID {plantId} does not exist, or does not belong to current user.");
            }

            var wateringEvents = await _wateringEventRepository.GetWateringEventsByPlantIdAsync(plantId, userId, pageNumber, pageSize);

            return _mapper.Map<PagedResponseDto<WateringEventResponseDto>>(wateringEvents);
        }

        public async Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEventDto, string userId)
        {
            var plantExists = await _plantsRepository.PlantExistsForUserAsync(wateringEventDto.PlantId, userId);
            if (!plantExists)
            {
                throw new KeyNotFoundException($"Plant with ID {wateringEventDto.PlantId} was not found.");
            }

            var entity = _mapper.Map<WateringEvent>(wateringEventDto);
            await _wateringEventRepository.AddWateringEventAsync(entity);

            return _mapper.Map<WateringEventResponseDto>(entity);
        }

        public async Task<bool> DeleteWateringEventAsync(int wateringEventId, string userId)
        {
            return await _wateringEventRepository.DeleteWateringEventAsync(wateringEventId, userId);
        }
    }
}
