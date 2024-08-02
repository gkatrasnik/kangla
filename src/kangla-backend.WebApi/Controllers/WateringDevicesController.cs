using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<IEnumerable<WateringDeviceResponseDto>>> GetWateringDevices(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var wateringDevices = await _wateringDeviceService.GetWateringDevicesAsync(pageNumber, pageSize);
            return Ok(wateringDevices);            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDevice(int id)
        {            
            var wateringDevice = await _wateringDeviceService.GetWateringDeviceAsync(id);
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
            var updatedDevice = await _wateringDeviceService.UpdateWateringDeviceAsync(id, wateringDevice);
            return Ok(updatedDevice);            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWateringDevice(int id)
        {            
            var deleted = await _wateringDeviceService.DeleteWateringDeviceAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Watering device with ID {id} not found." });
            }

            return NoContent();
        }
    }
}
