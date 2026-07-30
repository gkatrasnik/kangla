using AutoMapper;
using kangla.Application.Shared;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;

namespace kangla.Application.HumidityMeasurements
{
    /// <summary>
    /// Provides the authenticated app with paginated, historical humidity readings for a device.
    /// Device check-ins create readings through <c>WateringCommandService</c> instead.
    /// </summary>
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

        /// <summary>
        /// Verifies device ownership and returns its stored sensor-reading history.
        /// </summary>
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

    }
}
