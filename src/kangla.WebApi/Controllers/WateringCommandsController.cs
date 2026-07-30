using System.Security.Claims;
using kangla.Application.WateringCommands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kangla.WebApi.Controllers
{
    [Route("api/WateringDevices/{deviceId}/watering-commands")]
    [ApiController]
    [Authorize]
    public class WateringCommandsController : ControllerBase
    {
        private readonly IWateringCommandService _wateringCommandService;

        public WateringCommandsController(IWateringCommandService wateringCommandService)
        {
            _wateringCommandService = wateringCommandService;
        }

        [HttpPost]
        public async Task<ActionResult<WateringCommandResponseDto>> Create(int deviceId)
        {
            var userId = GetUserId();
            var (command, created) = await _wateringCommandService.CreateForUserAsync(deviceId, userId);
            if (!created)
            {
                return Ok(command);
            }

            return CreatedAtAction(nameof(Get), new { deviceId, commandId = command.Id }, command);
        }

        [HttpGet("{commandId}")]
        public async Task<ActionResult<WateringCommandResponseDto>> Get(int deviceId, int commandId)
        {
            var command = await _wateringCommandService.GetForUserAsync(deviceId, commandId, GetUserId());
            return Ok(command);
        }

        [HttpDelete("{commandId}")]
        public async Task<IActionResult> Cancel(int deviceId, int commandId)
        {
            try
            {
                await _wateringCommandService.CancelForUserAsync(deviceId, commandId, GetUserId());
                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");
        }
    }
}
