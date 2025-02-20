using System.ComponentModel.DataAnnotations;

namespace kangla.Application.Plants.DTO
{
    public class PlantCreateRequestDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name must be less than 50 characters long.")]
        public string Name { get; set; } = default!;
        [StringLength(100, ErrorMessage = "Scientific name must be less than 100 characters long.")]
        public string? ScientificName { get; set; } = default!;
        [StringLength(500, ErrorMessage = "Description must be less than 500 characters long.")]
        public string? Description { get; set; }
        [StringLength(100, ErrorMessage = "Location must be less than 100 characters long.")]
        public string? Location { get; set; }
        [StringLength(500, ErrorMessage = "Notes must be less than 500 characters long.")]
        public string? Notes { get; set; }
        [Required]
        [Range(1, 365, ErrorMessage = "Watering interval must be between 1 and 365 days.")]
        public int WateringInterval { get; set; } = 0;
        [StringLength(500, ErrorMessage = "Watering instructions must be less than 500 characters long.")]
        public string? WateringInstructions { get; set; } = default!;
        public Guid? ImageId { get; set; }
    }
}
