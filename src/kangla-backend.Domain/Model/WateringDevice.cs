using System.ComponentModel.DataAnnotations;

namespace Domain.Model
{
    public class WateringDevice
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters long.")]
        public string Name { get; set; } = default!;
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 100 characters long.")]
        public string? Description { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Location must be between 1 and 100 characters long.")]
        public string? Location { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Notes must be between 1 and 100 characters long.")]
        public string? Notes { get; set; }
        /// <summary>
        /// Whether the device is active - can be turned off
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Whether the device has been deleted - is not displayed in UI
        /// </summary>
        [Required]
        public bool Deleted { get; set; }
        public bool WaterNow { get; set; }
        public DateTime LastWatered { get; set; }
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
        [Range(1, 1000, ErrorMessage = "Interval must be between 1 and 1000 hours.")]
        public int WateringIntervalSetting { get; set; } = 420;
        /// <summary>
        /// Duration in seconds for watering duration
        /// </summary>
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 1000 seconds.")]
        public int WateringDurationSetting { get; set; } = 3;
        public List<WateringEvent>? WateringEvents { get; set; }
        public List<HumidityMeasurement>? HumidityMeasurements { get; set; }
        /// <summary>
        /// Device token by which user adds watering device to his account
        /// Should be written on device
        /// </summary>
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "DeviceToken must be 10 characters long.")]
        public string DeviceToken { get; set; } = default!;
    }
}
