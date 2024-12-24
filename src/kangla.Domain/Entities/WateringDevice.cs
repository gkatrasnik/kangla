using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class WateringDevice : IEntity
    {
        [Required]
        public string UserId { get; set; } = default!;
        /// <summary>
        /// Whether the device is active - can be turned off
        /// </summary>
        [Required]
        public bool WaterNow { get; set; }
        /// <summary>
        /// Minimum humidity to start watering 
        /// This is actual read value from capacitive humidity sensor
        /// </summary>
        [Required]
        [Range(250, 750, ErrorMessage = "Humidity reading must be between 250 and 750.")]
        public int MinimumSoilHumidity { get; set; } = 400;
        /// <summary>
        /// Interval in hours between watering events
        /// </summary>
        [Required]
        [Range(1, 365, ErrorMessage = "Interval must be between 1 and 1000 days.")]
        public int WateringIntervalSetting { get; set; } = 7;
        /// <summary>
        /// Duration in seconds for watering duration
        /// </summary>
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 1000 seconds.")]
        public int WateringDurationSetting { get; set; } = 3;
        public List<HumidityMeasurement>? HumidityMeasurements { get; set; }
        /// <summary>
        /// Device token by which user adds watering device to his account
        /// Should be written on device
        /// </summary>
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "DeviceToken must be 10 characters long.")]
        public string DeviceToken { get; set; } = default!;
        /// <summary>
        /// User id from Microsoft.AspNetCore.Identity that is owner of the device.
        /// </summary>
        [Required]
        public int PlantId { get; set; } = default!;
        public Plant Plant { get; set; } = default!;
    }
}
