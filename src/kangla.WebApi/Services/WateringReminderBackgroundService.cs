using kangla.Domain.Interfaces;

namespace kangla.WebApi.Services
{
    public class WateringReminderBackgroundService : BackgroundService
    {
        // The IANA identifier provides Central European standard and daylight-saving time.
        private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Ljubljana");
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<WateringReminderBackgroundService> _logger;

        public WateringReminderBackgroundService(IServiceScopeFactory scopeFactory, TimeProvider timeProvider, ILogger<WateringReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun();
                _logger.LogInformation("Next watering reminder run is in {Delay}", delay);
                await Task.Delay(delay, _timeProvider, stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IWateringReminderService>();
                    await service.SendDailyRemindersAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Daily watering reminder job failed.");
                }
            }
        }

        private TimeSpan GetDelayUntilNextRun()
        {
            var localNow = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), CentralEuropeanTimeZone);
            var nextRunDate = localNow.TimeOfDay >= TimeSpan.FromHours(11)
                ? localNow.Date.AddDays(1)
                : localNow.Date;
            var nextRunLocal = new DateTime(nextRunDate.Year, nextRunDate.Month, nextRunDate.Day, 11, 0, 0, DateTimeKind.Unspecified);
            var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRunLocal, CentralEuropeanTimeZone);
            return nextRunUtc - _timeProvider.GetUtcNow().UtcDateTime;
        }
    }
}
