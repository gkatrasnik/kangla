using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class HumidityMeasurementService : IHumidityMeasurementService
    {
        private readonly IHumidityMeasurementRepository _humidityMeasurementRepository;
        private readonly IWateringDeviceRepository _wateringDeviceRepository;
        private readonly IMapper _mapper;

        public HumidityMeasurementService(IHumidityMeasurementRepository humidityMeasurementRepository, IWateringDeviceRepository wateringDeviceRepository, IMapper mapper)
        {
            _humidityMeasurementRepository = humidityMeasurementRepository;
            _wateringDeviceRepository = wateringDeviceRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponseDto<HumidityMeasurementResponseDto>> GetHumidityMeasurementsForDeviceAsync(int deviceId, string userId, int pageNumber, int pageSize)
        {
            var deviceExists = await _wateringDeviceRepository.WateringDeviceExistsForUserAsync(deviceId, userId);
            if (!deviceExists)
            {
                throw new ArgumentException($"Device with ID {deviceId} does not exist, or does not belong to current user.");
            }

            var humidityMeasurements = await _humidityMeasurementRepository.GetHumidityMeasurementsByDeviceIdAsync(deviceId, userId, pageNumber, pageSize);

            return _mapper.Map<PagedResponseDto<HumidityMeasurementResponseDto>>(humidityMeasurements);            
        }

        public async Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {           
            var deviceExists = await _wateringDeviceRepository.WateringDeviceExistsAsync(humidityMeasurement.WateringDeviceId);
            if (!deviceExists)
            {
                throw new ArgumentException($"Device with ID {humidityMeasurement.WateringDeviceId} does not exist.");
            }

            var entity = _mapper.Map<HumidityMeasurement>(humidityMeasurement);
            await _humidityMeasurementRepository.AddHumidityMeasurementAsync(entity);

            return _mapper.Map<HumidityMeasurementResponseDto>(entity);            
        }
    }
}
