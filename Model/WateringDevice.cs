using System.ComponentModel.DataAnnotations;

namespace kangla_backend.Model
{
    public class WateringDevice
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public bool WaterNow { get; set; }
        public int WateringIntervalSetting { get; set; }
        public int WateringDurationSetting { get; set; }
        public List<WateringEvent> WateringEvents { get; set; } 
    }
}
