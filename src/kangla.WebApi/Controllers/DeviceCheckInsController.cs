using kangla.Application.WateringCommands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace kangla.WebApi.Controllers
{
    [Route("api/device/check-ins")]
    [ApiController]
    [EnableRateLimiting("device-api")]
    /// <summary>
    /// ESP32 entry point for periodic check-ins: submit an optional raw humidity reading and receive pending work.
    /// </summary>
    public class DeviceCheckInsController : ControllerBase
    {
        private readonly IWateringCommandService _wateringCommandService;

        public DeviceCheckInsController(IWateringCommandService wateringCommandService)
        {
            _wateringCommandService = wateringCommandService;
        }

        [HttpPost]
        /// <summary>
        /// Authenticates the device using its credential header and delegates the check-in protocol to the application service.
        /// </summary>
        public async Task<ActionResult<DeviceCheckInResponseDto>> CheckIn(
            DeviceCheckInRequestDto request,
            [FromHeader(Name = "X-Device-Credential")] string? deviceCredential)
        {
            var response = await _wateringCommandService.CheckInAsync(request, deviceCredential ?? string.Empty);
            return Ok(response);
        }
    }
}
