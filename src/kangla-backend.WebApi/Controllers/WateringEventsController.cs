using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Application.Services;
using Domain.Model;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WateringEventsController : ControllerBase
    {
        private readonly ILogger<WateringEventsController> _logger;
        private readonly IWateringEventService _wateringEventService;

        public WateringEventsController(ILogger<WateringEventsController> logger, IWateringEventService wateringEventService)
        {
            _logger = logger;
            _wateringEventService = wateringEventService;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<WateringEventResponseDto>>> GetWateringEventsForDevice(int deviceId)
        {
            var wateringEvents = await _wateringEventService.GetWateringEventsForDeviceAsync(deviceId);
            return Ok(wateringEvents);
        }

        [HttpPost]
        public async Task<ActionResult<WateringEventResponseDto>> PostWateringEvent(WateringEventCreateRequestDto wateringEvent)
        {
            try
            {
                var createdEvent = await _wateringEventService.CreateWateringEventAsync(wateringEvent);
                return CreatedAtAction(nameof(GetWateringEventsForDevice), new { deviceId = createdEvent.WateringDeviceId }, createdEvent);
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
    }
}
