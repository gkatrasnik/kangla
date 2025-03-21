using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using kangla.Application.WateringDevices;

namespace kangla.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WateringDevicesController : ControllerBase
    {
        private readonly IWateringDeviceService _wateringDeviceService;

        public WateringDevicesController(IWateringDeviceService wateringDeviceService)
        {
            _wateringDeviceService = wateringDeviceService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WateringDeviceResponseDto>>> GetWateringDevices(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var wateringDevices = await _wateringDeviceService.GetWateringDevicesAsync(userId, pageNumber, pageSize);
            return Ok(wateringDevices);
        }

        [Authorize]
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDevice(int deviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var wateringDevice = await _wateringDeviceService.GetWateringDeviceAsync(deviceId, userId);
            return Ok(wateringDevice);
        }

        [Authorize]
        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDeviceByPlantId(int plantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var wateringDevice = await _wateringDeviceService.GetWateringDeviceByPlantIdAsync(plantId, userId);
            return Ok(wateringDevice);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<WateringDeviceResponseDto>> PostWateringDevice(WateringDeviceCreateRequestDto wateringDevice)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var createdDevice = await _wateringDeviceService.CreateWateringDeviceAsync(wateringDevice, userId);
            return CreatedAtAction(nameof(GetWateringDevice), new { deviceId = createdDevice.Id }, createdDevice);
        }

        [Authorize]
        [HttpPut("{deviceId}")]
        public async Task<IActionResult> PutWateringDevice(int deviceId, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var updatedDevice = await _wateringDeviceService.UpdateWateringDeviceAsync(deviceId, userId, wateringDevice);
            return Ok(updatedDevice);
        }

        [Authorize]
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteWateringDevice(int deviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var deleted = await _wateringDeviceService.DeleteWateringDeviceAsync(deviceId, userId);
            if (!deleted)
            {
                return NotFound(new { message = $"Watering device with ID {deviceId} not found." });
            }

            return NoContent();
        }
    }
}
