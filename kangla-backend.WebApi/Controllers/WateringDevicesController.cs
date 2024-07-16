using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WateringDeviceResponseDto>>> GetWateringDevices()
        {
            var wateringDevices = await _wateringDeviceService.GetWateringDevicesAsync();
            return Ok(wateringDevices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDevice(int id)
        {
            var wateringDevice = await _wateringDeviceService.GetWateringDeviceAsync(id);
            if (wateringDevice == null)
            {
                return NotFound();
            }
            return Ok(wateringDevice);
        }

        [HttpPost]
        public async Task<ActionResult<WateringDeviceResponseDto>> PostWateringDevice(WateringDeviceCreateRequestDto wateringDevice)
        {
            var createdDevice = await _wateringDeviceService.CreateWateringDeviceAsync(wateringDevice);
            return CreatedAtAction(nameof(GetWateringDevice), new { id = createdDevice.Id }, createdDevice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWateringDevice(int id, WateringDeviceUpdateRequestDto wateringDevice)
        {
            try
            {
                var updatedDevice = await _wateringDeviceService.UpdateWateringDeviceAsync(id, wateringDevice);
                return Ok(updatedDevice);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWateringDevice(int id)
        {
            var deleted = await _wateringDeviceService.DeleteWateringDeviceAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
