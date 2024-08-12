using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WateringDevicesController : ControllerBase
    {
        private readonly ILogger<WateringDevicesController> _logger;
        private readonly IWateringDeviceService _wateringDeviceService;

        public WateringDevicesController(ILogger<WateringDevicesController> logger, IWateringDeviceService wateringDeviceService)
        {
            _logger = logger;
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
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDevice(int deviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var wateringDevice = await _wateringDeviceService.GetWateringDeviceAsync(deviceId, userId);
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

        //todo remove
        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            // Retrieve the values from environment variables
            var defaultFromEmail = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_DEFAULT_FROM_EMAIL");
            var host = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_HOST");
            var portString = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PORT");
            var username = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_USERNAME");
            var password = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PASSWORD");

            // Convert the port string to an integer, handle potential format issues
            if (!int.TryParse(portString, out var port))
            {
                port = 0; // Or handle the error appropriately
            }

            // Return the settings as a JSON object
            return Ok(new
            {
                DefaultFromEmail = defaultFromEmail,
                Host = host,
                Port = port,
                Username = username,
                Password = password
            });
        }
    }
}
