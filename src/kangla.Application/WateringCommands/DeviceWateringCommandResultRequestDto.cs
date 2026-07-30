using System.ComponentModel.DataAnnotations;

namespace kangla.Application.WateringCommands
{
    public class DeviceWateringCommandResultRequestDto
    {
        [Required]
        public WateringCommandOutcome? Outcome { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }
        [StringLength(200)]
        public string? FailureReason { get; set; }
    }
}
