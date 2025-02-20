using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities

{
    public class Plant : IEntity
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// User id from Microsoft.AspNetCore.Identity that is owner of the plant.
        /// </summary>
        [Required]        
        public string UserId { get; set; } = default!;
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
        /// <summary>
        /// WateringInterval unit is days.
        /// </summary>
        public int WateringInterval { get; set; } = 0;
        [StringLength(500, ErrorMessage = "Watering instructions must be less than 500 characters long.")]
        public string? WateringInstructions { get; set; } = default!;
        public List<WateringEvent>? WateringEvents { get; set; }        
        public WateringDevice? WateringDevice { get; set; }        
        public Guid? ImageId { get; set; } = default!;
    }
}
