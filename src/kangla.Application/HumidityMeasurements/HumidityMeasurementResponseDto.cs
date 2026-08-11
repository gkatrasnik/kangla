using System.ComponentModel.DataAnnotations;

namespace kangla.Application.HumidityMeasurements
{
    public class HumidityMeasurementResponseDto
    {
        public required int Id { get; set; }
        public required DateTime DateTime { get; set; }
        public int RawSoilMoisture { get; set; }
        public int? SoilMoisturePercentage { get; set; }
        public int WateringDeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
