namespace kangla.Domain.Interfaces
{
    public interface IWateringReminderService
    {
        Task<int> SendDailyRemindersAsync(CancellationToken cancellationToken = default);
    }
}
