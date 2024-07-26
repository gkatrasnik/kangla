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
            try
            {
                var wateringEvents = await _wateringEventRepository.GetWateringEventsByDeviceIdAsync(deviceId);

                if (wateringEvents == null)
                {
                    return Enumerable.Empty<WateringEventResponseDto>();
                }

                return _mapper.Map<IEnumerable<WateringEventResponseDto>>(wateringEvents);
            }
            catch (ArgumentException ex)
            {
                // Log 
                throw new ArgumentException("The requested watering events could not be found.", ex);
            }
            catch (Exception ex)
            {
                // Log 
                throw new Exception("An error occurred while retrieving watering events.", ex);
            }
        }

        public async Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent)
        {
            try
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
            catch (ArgumentException ex)
            {
                // log
                throw new ArgumentException("Validation error occurred: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // log
                throw new Exception("An error occurred while creating the watering event.", ex);
            }
        }
    }
}
