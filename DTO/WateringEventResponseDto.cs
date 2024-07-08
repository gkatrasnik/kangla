using kangla_backend.Model;
using System.ComponentModel.DataAnnotations;

namespace kangla_backend.DTO
{
    public class WateringEventResponseDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
    }
}
