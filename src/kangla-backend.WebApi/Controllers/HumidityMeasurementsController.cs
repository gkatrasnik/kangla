using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Microsoft.AspNetCore.Authorization;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HumidityMeasurementsController : ControllerBase
    {
        private readonly ILogger<HumidityMeasurementsController> _logger;
        private readonly IHumidityMeasurementService _humidityMeasurementService;

        public HumidityMeasurementsController(ILogger<HumidityMeasurementsController> logger, IHumidityMeasurementService humidityMeasurementService)
        {
            _logger = logger;
            _humidityMeasurementService = humidityMeasurementService;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<HumidityMeasurementResponseDto>>> GetHumidityMeasurementsForDevice(int deviceId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var humidityMeasurements = await _humidityMeasurementService.GetHumidityMeasurementsForDeviceAsync(deviceId, pageNumber, pageSize);           
            return Ok(humidityMeasurements);
        }

        [HttpPost]
        public async Task<ActionResult<HumidityMeasurementResponseDto>> PostHumidityMeasurement(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {            
            var createdMeasurement = await _humidityMeasurementService.CreateHumidityMeasurementAsync(humidityMeasurement);
            return CreatedAtAction(nameof(GetHumidityMeasurementsForDevice), new { deviceId = createdMeasurement.WateringDeviceId }, createdMeasurement);
        }
    }
}
