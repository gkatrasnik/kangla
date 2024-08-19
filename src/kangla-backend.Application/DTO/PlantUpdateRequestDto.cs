using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class PlantUpdateRequestDto
    {
        [Required]
        [StringLength(30, ErrorMessage = "Name must be less than 30 characters long.")]
        public string Name { get; set; } = default!;
        [StringLength(50, ErrorMessage = "Scientific name must be less than 50 characters long.")]
        public string? ScientificName { get; set; } = default!;
        [StringLength(100, ErrorMessage = "Description must be less than 100 characters long.")]
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
        public int? WateringDeviceId { get; set; }
        public IFormFile? Image { get; set; }
        public bool removeImage { get; set; }
    }
}
