using System.Net;
using System.Text;
using kangla.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace kangla.Infrastructure.Services
{
    public class WateringReminderService : IWateringReminderService
    {
        // The IANA identifier provides Central European standard and daylight-saving time.
        private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Ljubljana");
        private readonly PlantsContext _context;
        private readonly IEmailService _emailService;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<WateringReminderService> _logger;

        public WateringReminderService(
            PlantsContext context,
            IEmailService emailService,
            TimeProvider timeProvider,
            ILogger<WateringReminderService> logger)
        {
            _context = context;
            _emailService = emailService;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<int> SendDailyRemindersAsync(CancellationToken cancellationToken = default)
        {
            var localNow = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), CentralEuropeanTimeZone).DateTime;
            var localDate = DateOnly.FromDateTime(localNow);
            var recipients = await _context.UserNotificationPreferences
                .Where(p => p.WateringReminderEmailsEnabled &&
                    (p.LastWateringReminderSentOn == null || p.LastWateringReminderSentOn != localDate))
                .Join(_context.Users,
                    preference => preference.UserId,
                    user => user.Id,
                    (preference, user) => new { Preference = preference, User = user })
                .Where(x => x.User.EmailConfirmed && x.User.Email != null)
                .ToListAsync(cancellationToken);

            var sentCount = 0;
            foreach (var recipient in recipients)
            {
                try
                {
                    var plants = await _context.Plants.AsNoTracking()
                        .Where(p => p.UserId == recipient.User.Id)
                        .Select(p => new DuePlant(
                            p.Name,
                            p.Location,
                            p.WateringInterval,
                            p.WateringEvents!.OrderByDescending(e => e.Start).Select(e => (DateTime?)e.Start).FirstOrDefault()))
                        .ToListAsync(cancellationToken);

                    var duePlants = plants.Where(p => p.LastWatered is null || p.LastWatered.Value.AddDays(p.WateringInterval) < localNow)
                        .OrderBy(p => p.Name)
                        .ToList();

                    if (duePlants.Count == 0)
                    {
                        continue;
                    }

                    var body = BuildEmailBody(duePlants, localNow);
                    await _emailService.Send(new kangla.Domain.Model.EmailMessage(
                        recipient.User.Email!,
                        duePlants.Count == 1 ? "A plant needs watering" : $"{duePlants.Count} plants need watering",
                        body));

                    recipient.Preference.LastWateringReminderSentOn = localDate;
                    await _context.SaveChangesAsync(cancellationToken);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not send watering reminder email to user {UserId}", recipient.User.Id);
                }
            }

            _logger.LogInformation("Completed daily watering reminders. Sent {SentCount} emails.", sentCount);
            return sentCount;
        }

        private static string BuildEmailBody(IReadOnlyCollection<DuePlant> duePlants, DateTime localNow)
        {
            var rows = new StringBuilder();
            foreach (var plant in duePlants)
            {
                var name = WebUtility.HtmlEncode(plant.Name);
                var location = plant.Location is string value && !string.IsNullOrWhiteSpace(value)
                    ? $" <span style=\"color:#666\">({WebUtility.HtmlEncode(value)})</span>"
                    : string.Empty;
                var status = plant.LastWatered is DateTime lastWatered
                    ? $"Overdue by {Math.Max(1, (localNow.Date - lastWatered.AddDays(plant.WateringInterval).Date).Days)} day(s)"
                    : "Needs first watering";
                rows.Append($"<li><strong>{name}</strong>{location} — {status}</li>");
            }

            return $"<h1>Watering reminder</h1><p>Your plants need some attention today:</p><ul>{rows}</ul><p>Open Kangla to mark them as watered.</p>";
        }

        private sealed record DuePlant(string Name, string? Location, int WateringInterval, DateTime? LastWatered);
    }
}
