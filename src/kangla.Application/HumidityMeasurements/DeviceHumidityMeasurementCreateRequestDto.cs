using System.ComponentModel.DataAnnotations;

namespace kangla.Application.HumidityMeasurements
{
    /// <summary>
    /// Measurement payload accepted from a physical watering device.
    /// The target device is derived from the credential, never from request input.
    /// </summary>
    public class DeviceHumidityMeasurementCreateRequestDto
    {
        [Required]
        public required DateTime DateTime { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Value must be between 0 and 1000")]
        public int SoilHumidity { get; set; }
    }
}
