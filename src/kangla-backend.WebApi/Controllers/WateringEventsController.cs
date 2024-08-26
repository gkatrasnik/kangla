using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WateringEventsController : ControllerBase
    {
        private readonly IWateringEventService _wateringEventService;

        public WateringEventsController(IWateringEventService wateringEventService)
        {
            _wateringEventService = wateringEventService;
        }

        [Authorize]
        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<PagedResponseDto<WateringEventResponseDto>>> GetWateringEventsForPlant(int plantId, int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var wateringEvents = await _wateringEventService.GetWateringEventsForPlantAsync(plantId, userId, pageNumber, pageSize);
            return Ok(wateringEvents);           
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<WateringEventResponseDto>> PostWateringEvent(WateringEventCreateRequestDto wateringEvent)
        {            
            var createdEvent = await _wateringEventService.CreateWateringEventAsync(wateringEvent);
            return CreatedAtAction(nameof(GetWateringEventsForPlant), new { plantId = createdEvent.PlantId }, createdEvent);          
        }
    }
}
