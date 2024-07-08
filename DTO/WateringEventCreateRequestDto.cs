using kangla_backend.Model;
using System.ComponentModel.DataAnnotations;

namespace kangla_backend.DTO
{
    public class WateringEventRequestDto
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
    }
}
