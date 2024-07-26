using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Model;


namespace Application.Services
{
    public class WateringEventService : IWateringEventService
    {
        private readonly IWateringEventRepository _wateringEventRepository;
        private readonly IWateringDeviceRepository _wateringDeviceRepository;
        private readonly IMapper _mapper;

        public WateringEventService(IWateringEventRepository wateringEventRepository, IWateringDeviceRepository wateringDeviceRepository, IMapper mapper)
        {
            _wateringEventRepository = wateringEventRepository;
            _wateringDeviceRepository = wateringDeviceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WateringEventResponseDto>> GetWateringEventsForDeviceAsync(int deviceId)
        {
            var wateringEvents = await _wateringEventRepository.GetWateringEventsByDeviceIdAsync(deviceId);
            return _mapper.Map<IEnumerable<WateringEventResponseDto>>(wateringEvents);
        }

        public async Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent)
        {
            var deviceExists = _wateringDeviceRepository.WateringDeviceExists(wateringEvent.WateringDeviceId);
            if (!deviceExists)
            {
                throw new ArgumentException($"Device with ID {wateringEvent.WateringDeviceId} does not exist.");
            }

            var entity = _mapper.Map<WateringEvent>(wateringEvent);
            await _wateringEventRepository.AddWateringEventAsync(entity);
            return _mapper.Map<WateringEventResponseDto>(entity);
        }
    }
}
