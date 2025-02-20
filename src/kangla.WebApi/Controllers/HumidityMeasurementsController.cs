using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using kangla.Application.HumidityMeasurements;

namespace kangla.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HumidityMeasurementsController : ControllerBase
    {
        private readonly IHumidityMeasurementService _humidityMeasurementService;

        public HumidityMeasurementsController(IHumidityMeasurementService humidityMeasurementService)
        {
            _humidityMeasurementService = humidityMeasurementService;
        }

        [Authorize]
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<HumidityMeasurementResponseDto>>> GetHumidityMeasurementsForDevice(int deviceId, int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var humidityMeasurements = await _humidityMeasurementService.GetHumidityMeasurementsForDeviceAsync(deviceId, userId, pageNumber, pageSize);
            return Ok(humidityMeasurements);
        }

        [Authorize] //this will be authorized by the api key
        [HttpPost]
        public async Task<ActionResult<HumidityMeasurementResponseDto>> PostHumidityMeasurement(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {
            var createdMeasurement = await _humidityMeasurementService.CreateHumidityMeasurementAsync(humidityMeasurement);
            return CreatedAtAction(nameof(GetHumidityMeasurementsForDevice), new { deviceId = createdMeasurement.WateringDeviceId }, createdMeasurement);
        }
    }
}
