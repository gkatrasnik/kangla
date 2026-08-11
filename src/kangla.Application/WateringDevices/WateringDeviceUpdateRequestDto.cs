using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace kangla.Application.WateringDevices
{
    public class WateringDeviceUpdateRequestDto
    {
        // Stored for future automatic watering; it is not currently evaluated by the API or device protocol.
        [Required]
        [Range(1, 365, ErrorMessage = "Interval must be between 1 and 365 days.")]
        public int WateringIntervalSetting { get; set; }
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 seconds.")]
        public int WateringDurationSetting { get; set; }
        public int? PlantId { get; set; }
    }
}
