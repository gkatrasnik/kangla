using kangla_backend.Model;
using System.ComponentModel.DataAnnotations;

namespace kangla_backend.DTO
{
    public class HumidityMeasurementResponseDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required DateTime DateTime { get; set; }
        [Required]
        [Range(0, 1000, ErrorMessage = "Value must be between 0 and 1000")]
        public int SoilHumidity { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
    }
}
