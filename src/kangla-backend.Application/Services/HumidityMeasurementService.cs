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
            try
            {
                var humidityMeasurements = await _humidityMeasurementRepository.GetHumidityMeasurementsByDeviceIdAsync(deviceId);

                if (humidityMeasurements == null)
                {
                    return Enumerable.Empty<HumidityMeasurementResponseDto>();
                }

                return _mapper.Map<IEnumerable<HumidityMeasurementResponseDto>>(humidityMeasurements);
            }
            catch (ArgumentException ex)
            {
                // log
                throw new ArgumentException("Invalid device ID or other argument-related issue.", ex);
            }
            catch (Exception ex)
            {
                // log
                throw new Exception("An error occurred while retrieving humidity measurements.", ex);
            }
        }

        public async Task<HumidityMeasurementResponseDto> CreateHumidityMeasurementAsync(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {
            try
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
            catch (ArgumentException ex)
            {
                // Log specific exception and rethrow
                throw new ArgumentException("Validation error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions and rethrow
                throw new Exception("An error occurred while creating humidity measurement.", ex);
            }
        }
    }
}
