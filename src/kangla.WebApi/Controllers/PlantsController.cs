using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using kangla.Application.DTO;
using kangla.Application.Interfaces;

namespace kangla.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly IPlantsService _plantsService;

        public PlantsController(IPlantsService plantsService)
        {
            _plantsService = plantsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<PlantResponseDto>>> GetPlants(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }
            var plants = await _plantsService.GetPlantsAsync(userId, pageNumber, pageSize);
            return Ok(plants);
        }

        [Authorize]
        [HttpGet("{plantId}")]
        public async Task<ActionResult<PlantResponseDto>> GetPlant(int plantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var plant = await _plantsService.GetPlantAsync(plantId, userId);
            return Ok(plant);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PlantResponseDto>> PostPlant(PlantCreateRequestDto plantDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var createdPlant = await _plantsService.CreatePlantAsync(plantDto, userId);
            return CreatedAtAction(nameof(GetPlant), new { plantId = createdPlant.Id }, createdPlant);
        }

        [Authorize]
        [HttpPut("{plantId}")]
        public async Task<IActionResult> PutPlant(int plantId, PlantUpdateRequestDto plantDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var updatedPlant = await _plantsService.UpdatePlantAsync(plantId, userId, plantDto);
            return Ok(updatedPlant);
        }

        [Authorize]
        [HttpDelete("{plantId}")]
        public async Task<IActionResult> DeletePlant(int plantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");

            var deleted = await _plantsService.DeletePlantAsync(plantId, userId);
            if (!deleted)
            {
                return NotFound(new { message = $"Plant with ID {plantId} not found." });
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("recognize")]
        public async Task<ActionResult<PlantRecognizeResponseDto>> RecognizePlant(PlantRecognizeRequestDto plantRecognizeDto)
        {
            var recognizedPlant = await _plantsService.RecognizePlantAsync(plantRecognizeDto);
            return Ok(recognizedPlant);
        }
    }
}
