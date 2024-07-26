using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Model;

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

        public async Task<IEnumerable<HumidityMeasurementResponseDto>> GetHumidityMeasurementsForDeviceAsync(int deviceId)
        {
            var humidityMeasurements = await _humidityMeasurementRepository.GetHumidityMeasurementsByDeviceIdAsync(deviceId);
            return _mapper.Map<IEnumerable<HumidityMeasurementResponseDto>>(humidityMeasurements);
        }

        public async Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {
            var deviceExists = _wateringDeviceRepository.WateringDeviceExists(humidityMeasurement.WateringDeviceId);
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
