using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;


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
        public async Task<ActionResult<IEnumerable<HumidityMeasurementResponseDto>>> GetHumidityMeasurementsForDevice(int deviceId)
        {           
            var humidityMeasurements = await _humidityMeasurementService.GetHumidityMeasurementsForDeviceAsync(deviceId);
            if (humidityMeasurements == null || !humidityMeasurements.Any())
            {
                return NotFound(new { message = $"No humidity measurements found for device with ID {deviceId}." });
            }

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
