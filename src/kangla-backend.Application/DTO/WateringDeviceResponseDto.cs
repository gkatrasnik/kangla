using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class WateringDeviceResponseDto
    {
        public int Id { get; set; }        
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool WaterNow { get; set; }
        public int MinimumSoilHumidity { get; set; } = 400;
        public int WateringIntervalSetting { get; set; } = 420;
        public int WateringDurationSetting { get; set; } = 3;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
