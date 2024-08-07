using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<WateringEventResponseDto>>> GetWateringEventsForDevice(int deviceId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var wateringEvents = await _wateringEventService.GetWateringEventsForDeviceAsync(deviceId, pageNumber, pageSize);
            return Ok(wateringEvents);           
        }

        [Authorize] //this will be authorized by the api key
        [HttpPost]
        public async Task<ActionResult<WateringEventResponseDto>> PostWateringEvent(WateringEventCreateRequestDto wateringEvent)
        {            
            var createdEvent = await _wateringEventService.CreateWateringEventAsync(wateringEvent);
            return CreatedAtAction(nameof(GetWateringEventsForDevice), new { deviceId = createdEvent.WateringDeviceId }, createdEvent);          
        }
    }
}
