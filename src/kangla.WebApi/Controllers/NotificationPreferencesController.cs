using System.Security.Claims;
using kangla.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kangla.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notification-preferences")]
    public class NotificationPreferencesController : ControllerBase
    {
        private readonly INotificationPreferenceService _preferences;

        public NotificationPreferencesController(INotificationPreferenceService preferences)
        {
            _preferences = preferences;
        }

        [HttpGet]
        public async Task<ActionResult<NotificationPreferencesResponse>> Get()
        {
            var enabled = await _preferences.GetWateringReminderEmailsEnabledAsync(GetUserId());
            return Ok(new NotificationPreferencesResponse(enabled));
        }

        [HttpPut("watering-reminder-emails")]
        public async Task<ActionResult<NotificationPreferencesResponse>> Put(WateringReminderEmailsRequest request)
        {
            var enabled = await _preferences.SetWateringReminderEmailsEnabledAsync(GetUserId(), request.Enabled);
            return Ok(new NotificationPreferencesResponse(enabled));
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID could not be retrieved from the token.");
    }

    public record NotificationPreferencesResponse(bool WateringReminderEmailsEnabled);
    public record WateringReminderEmailsRequest(bool Enabled);
}
