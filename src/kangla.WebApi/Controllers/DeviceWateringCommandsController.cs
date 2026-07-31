using kangla.Application.WateringCommands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace kangla.WebApi.Controllers
{
    [Route("api/device/watering-commands")]
    [ApiController]
    [EnableRateLimiting("device-api")]
    public class DeviceWateringCommandsController : ControllerBase
    {
        private readonly IWateringCommandService _wateringCommandService;

        public DeviceWateringCommandsController(IWateringCommandService wateringCommandService)
        {
            _wateringCommandService = wateringCommandService;
        }

        [HttpPost("{commandId}/acknowledgements")]
        public async Task<ActionResult<WateringCommandResponseDto>> Acknowledge(
            int commandId,
            [FromHeader(Name = "X-Device-Access-Key")] string? deviceAccessKey)
        {
            try
            {
                var command = await _wateringCommandService.AcknowledgeAsync(commandId, deviceAccessKey ?? string.Empty);
                return Ok(command);
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpPost("{commandId}/results")]
        public async Task<ActionResult<WateringCommandResponseDto>> ReportResult(
            int commandId,
            DeviceWateringCommandResultRequestDto request,
            [FromHeader(Name = "X-Device-Access-Key")] string? deviceAccessKey)
        {
            try
            {
                var command = await _wateringCommandService.ReportResultAsync(commandId, request, deviceAccessKey ?? string.Empty);
                return Ok(command);
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }
    }
}
