using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class UserNotificationPreference : IEntity
    {
        [Key]
        public string UserId { get; set; } = default!;
        public bool WateringReminderEmailsEnabled { get; set; }
        public DateOnly? LastWateringReminderSentOn { get; set; }
    }
}
