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
        public async Task<ActionResult<IEnumerable<WateringDeviceResponseDto>>> GetWateringDevices()
        {
            try
            {
                var wateringDevices = await _wateringDeviceService.GetWateringDevicesAsync();
                return Ok(wateringDevices);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WateringDeviceResponseDto>> GetWateringDevice(int id)
        {
            try
            {
                var wateringDevice = await _wateringDeviceService.GetWateringDeviceAsync(id);
                return Ok(wateringDevice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WateringDeviceResponseDto>> PostWateringDevice(WateringDeviceCreateRequestDto wateringDevice)
        {
            try
            {
                var createdDevice = await _wateringDeviceService.CreateWateringDeviceAsync(wateringDevice);
                return CreatedAtAction(nameof(GetWateringDevice), new { id = createdDevice.Id }, createdDevice);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Validation error: " + ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWateringDevice(int id, WateringDeviceUpdateRequestDto wateringDevice)
        {
            try
            {
                var updatedDevice = await _wateringDeviceService.UpdateWateringDeviceAsync(id, wateringDevice);
                return Ok(updatedDevice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWateringDevice(int id)
        {
            try
            {
                var deleted = await _wateringDeviceService.DeleteWateringDeviceAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Watering device with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
