using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class WateringEventResponseDto
    {
        public required int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int WateringDeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
