using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class WateringEvent : IEntity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = default!;
    }
}
