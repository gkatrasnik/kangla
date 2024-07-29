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
            if (wateringEvents == null || !wateringEvents.Any())
            {
                return NotFound(new { message = $"No humidity measurements found for device with ID {deviceId}." });
            }
            return Ok(wateringEvents);           
        }

        [HttpPost]
        public async Task<ActionResult<WateringEventResponseDto>> PostWateringEvent(WateringEventCreateRequestDto wateringEvent)
        {            
            var createdEvent = await _wateringEventService.CreateWateringEventAsync(wateringEvent);
            return CreatedAtAction(nameof(GetWateringEventsForDevice), new { deviceId = createdEvent.WateringDeviceId }, createdEvent);          
        }
    }
}
