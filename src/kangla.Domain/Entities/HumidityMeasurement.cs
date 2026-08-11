using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class HumidityMeasurement : IEntity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required DateTime DateTime { get; set; }
        /// <summary>
        /// Raw 12-bit ADC reading reported by the capacitive sensor.
        /// </summary>
        [Required]
        [Range(0, 4095, ErrorMessage = "Raw soil moisture must be between 0 and 4095.")]
        public int RawSoilMoisture { get; set; }
        /// <summary>
        /// Device-calibrated soil moisture percentage. Null for legacy raw-only readings.
        /// </summary>
        [Range(0, 100, ErrorMessage = "Soil moisture percentage must be between 0 and 100.")]
        public int? SoilMoisturePercentage { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
        public WateringDevice WateringDevice { get; set; } = default!;
    }
}
