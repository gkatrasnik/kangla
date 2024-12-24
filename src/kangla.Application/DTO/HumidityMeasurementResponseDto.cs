using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class HumidityMeasurementResponseDto
    {
        public required int Id { get; set; }
        public required DateTime DateTime { get; set; }
        public int SoilHumidity { get; set; }
        public int WateringDeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
