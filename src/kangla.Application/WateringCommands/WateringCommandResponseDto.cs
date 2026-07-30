using kangla.Domain.Entities;

namespace kangla.Application.WateringCommands
{
    public class WateringCommandResponseDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public WateringCommandStatus Status { get; set; }
        public int DurationSeconds { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? AcknowledgedAtUtc { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }
        public string? FailureReason { get; set; }
        public int? WateringEventId { get; set; }
    }
}
