using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Model;


namespace Application.Services
{
    public class WateringEventService : IWateringEventService
    {
        private readonly IWateringEventRepository _repository;
        private readonly IMapper _mapper;

        public WateringEventService(IWateringEventRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WateringEventResponseDto>> GetWateringEventsForDeviceAsync(int deviceId)
        {
            var wateringEvents = await _repository.GetWateringEventsByDeviceIdAsync(deviceId);
            return _mapper.Map<IEnumerable<WateringEventResponseDto>>(wateringEvents);
        }

        public async Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent)
        {
            var entity = _mapper.Map<WateringEvent>(wateringEvent);
            await _repository.AddWateringEventAsync(entity);
            return _mapper.Map<WateringEventResponseDto>(entity);
        }
    }
}
