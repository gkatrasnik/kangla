using kangla.Application.HumidityMeasurements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace kangla.WebApi.Controllers
{
    [Route("api/device/humidity-measurements")]
    [ApiController]
    [EnableRateLimiting("device-ingestion")]
    public class DeviceHumidityMeasurementsController : ControllerBase
    {
        private readonly IHumidityMeasurementService _humidityMeasurementService;

        public DeviceHumidityMeasurementsController(IHumidityMeasurementService humidityMeasurementService)
        {
            _humidityMeasurementService = humidityMeasurementService;
        }

        [HttpPost]
        public async Task<ActionResult<HumidityMeasurementResponseDto>> PostHumidityMeasurement(
            DeviceHumidityMeasurementCreateRequestDto humidityMeasurement,
            [FromHeader(Name = "X-Device-Credential")] string? deviceCredential)
        {
            var createdMeasurement = await _humidityMeasurementService.CreateDeviceHumidityMeasurementAsync(
                humidityMeasurement,
                deviceCredential ?? string.Empty);

            return Created($"/api/HumidityMeasurements/device/{createdMeasurement.WateringDeviceId}", createdMeasurement);
        }
    }
}
