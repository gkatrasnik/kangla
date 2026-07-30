using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class WateringDevice : IEntity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = default!;
        /// <summary>
        /// Reserved for a future automatic-watering mode.
        /// This is a raw value from a capacitive humidity sensor.
        /// </summary>
        [Required]
        [Range(250, 750, ErrorMessage = "Humidity reading must be between 250 and 750.")]
        public int MinimumSoilHumidity { get; set; } = 400;
        /// <summary>
        /// Reserved for a future automatic-watering mode. Unit is days.
        /// </summary>
        [Required]
        [Range(1, 365, ErrorMessage = "Interval must be between 1 and 365 days.")]
        public int WateringIntervalSetting { get; set; } = 7;
        /// <summary>
        /// Pump duration in seconds for a manual watering command.
        /// </summary>
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 seconds.")]
        public int WateringDurationSetting { get; set; } = 3;
        public List<HumidityMeasurement>? HumidityMeasurements { get; set; }
        public List<WateringCommand>? WateringCommands { get; set; }
        /// <summary>
        /// Device token by which user adds watering device to his account
        /// Should be written on device
        /// </summary>
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "DeviceToken must be 10 characters long.")]
        public string DeviceToken { get; set; } = default!;
        /// <summary>
        /// SHA-256 hash of the long-lived credential used by the physical device.
        /// A null value identifies a legacy device that has not been re-provisioned yet.
        /// </summary>
        [StringLength(64)]
        public string? DeviceCredentialHash { get; set; }
        /// <summary>
        /// User id from Microsoft.AspNetCore.Identity that is owner of the device.
        /// </summary>
        [Required]
        public int PlantId { get; set; } = default!;
        public Plant Plant { get; set; } = default!;
    }
}
