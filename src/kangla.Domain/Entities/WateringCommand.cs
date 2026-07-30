using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class WateringCommand : IEntity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
        public WateringDevice WateringDevice { get; set; } = default!;
        [Required]
        public WateringCommandStatus Status { get; set; } = WateringCommandStatus.Pending;
        [Required]
        [Range(1, 60)]
        public int DurationSeconds { get; set; }
        [Required]
        public DateTime RequestedAtUtc { get; set; }
        [Required]
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? AcknowledgedAtUtc { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }
        [StringLength(200)]
        public string? FailureReason { get; set; }
        public int? WateringEventId { get; set; }
        public WateringEvent? WateringEvent { get; set; }
    }
}
