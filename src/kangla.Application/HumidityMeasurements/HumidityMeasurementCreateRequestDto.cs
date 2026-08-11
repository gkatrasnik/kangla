using System.ComponentModel.DataAnnotations;

namespace kangla.Application.HumidityMeasurements
{
    public class HumidityMeasurementCreateRequestDto
    {
        [Required]
        public required DateTime DateTime { get; set; }
        [Required]
        [Range(0, 4095, ErrorMessage = "Raw soil moisture must be between 0 and 4095.")]
        public int RawSoilMoisture { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Soil moisture percentage must be between 0 and 100.")]
        public int SoilMoisturePercentage { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
    }
}
