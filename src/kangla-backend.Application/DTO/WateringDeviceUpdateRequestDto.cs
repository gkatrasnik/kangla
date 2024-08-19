using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class WateringDeviceUpdateRequestDto
    {        
        public bool WaterNow { get; set; }
        [Required]
        [Range(250, 750, ErrorMessage = "Humidity reading must be between 250 and 750.")]
        public int MinimumSoilHumidity { get; set; }
        [Required]
        [Range(1, 365, ErrorMessage = "Interval must be between 1 and 365 days.")]
        public int WateringIntervalSetting { get; set; }
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 seconds.")]
        public int WateringDurationSetting { get; set; }
        public int PlantId { get; set; } = default!;
    }
}
