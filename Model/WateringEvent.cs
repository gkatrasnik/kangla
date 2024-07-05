using System.ComponentModel.DataAnnotations;

namespace kangla_backend.Model
{
    public class WateringEvent
    {
        [Required]
        public required int Id  { get; set; }

        [Required]
        public int WateringDeviceId { get; set; }
        public WateringDevice WateringDevice { get; set; }
        [Required]
        public required DateTime DateTime { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public double SoilHumidity { get; set; }
    }
}
