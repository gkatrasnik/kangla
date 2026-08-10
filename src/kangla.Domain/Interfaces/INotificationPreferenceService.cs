namespace kangla.Domain.Interfaces
{
    public interface INotificationPreferenceService
    {
        Task<bool> GetWateringReminderEmailsEnabledAsync(string userId);
        Task<bool> SetWateringReminderEmailsEnabledAsync(string userId, bool enabled);
    }
}
