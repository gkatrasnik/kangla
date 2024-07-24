using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class WateringDeviceResponseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters long.")]
        public string Name { get; set; } = default!;
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 100 characters long.")]
        public string? Description { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Location must be between 1 and 100 characters long.")]
        public string? Location { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Notes must be between 1 and 100 characters long.")]
        public string? Notes { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public bool WaterNow { get; set; }
        [Required]
        [Range(250, 750, ErrorMessage = "Humidity reading must be between 250 and 750.")]
        public int MinimumSoilHumidity { get; set; } = 400;
        [Required]
        [Range(1, 1000, ErrorMessage = "Interval must be between 1 and 1000 hours.")]
        public int WateringIntervalSetting { get; set; } = 420;
        [Required]
        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 1000 seconds.")]
        public int WateringDurationSetting { get; set; } = 3;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
