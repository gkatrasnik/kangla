using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

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

        public async Task<PagedResponseDto<WateringEventResponseDto>> GetWateringEventsForDeviceAsync(int deviceId,string userId, int pageNumber, int pageSize)
        {
            var deviceExists = await _wateringDeviceRepository.WateringDeviceExistsForUserAsync(deviceId, userId);
            if (!deviceExists)
            {
                throw new ArgumentException($"Device with ID {deviceId} does not exist, or does not belong to current user.");
            }

            var wateringEvents = await _wateringEventRepository.GetWateringEventsByDeviceIdAsync(deviceId, userId, pageNumber, pageSize);           

            return _mapper.Map<PagedResponseDto<WateringEventResponseDto>>(wateringEvents);            
        }

        public async Task<WateringEventResponseDto> CreateWateringEventAsync(WateringEventCreateRequestDto wateringEvent)
        {
            var deviceExists = await _wateringDeviceRepository.WateringDeviceExistsAsync(wateringEvent.WateringDeviceId);
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
