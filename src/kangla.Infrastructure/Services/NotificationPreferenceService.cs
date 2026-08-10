using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Services
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly PlantsContext _context;

        public NotificationPreferenceService(PlantsContext context)
        {
            _context = context;
        }

        public async Task<bool> GetWateringReminderEmailsEnabledAsync(string userId)
        {
            return await _context.UserNotificationPreferences.AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => p.WateringReminderEmailsEnabled)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SetWateringReminderEmailsEnabledAsync(string userId, bool enabled)
        {
            var preference = await _context.UserNotificationPreferences
                .SingleOrDefaultAsync(p => p.UserId == userId);

            if (preference is null)
            {
                preference = new UserNotificationPreference { UserId = userId };
                _context.UserNotificationPreferences.Add(preference);
            }

            preference.WateringReminderEmailsEnabled = enabled;
            await _context.SaveChangesAsync();
            return preference.WateringReminderEmailsEnabled;
        }
    }
}
