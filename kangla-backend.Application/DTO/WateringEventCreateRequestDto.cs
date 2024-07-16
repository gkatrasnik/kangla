using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class WateringEventCreateRequestDto
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int WateringDeviceId { get; set; }
    }
}
