using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class WateringDevice : IEntity
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Null until the device has been claimed by a user.
        /// </summary>
        public string? UserId { get; set; }
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
        /// SHA-256 hash of the device access key printed on the physical device.
        /// The access key itself is never persisted.
        /// </summary>
        [StringLength(64)]
        public string? DeviceAccessKeyHash { get; set; }
        /// <summary>
        /// True after the device has been removed from a user's inventory. The record is retained for command and measurement history.
        /// </summary>
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        /// <summary>
        /// User id from Microsoft.AspNetCore.Identity that is owner of the device.
        /// </summary>
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
    }
}
