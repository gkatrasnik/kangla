namespace Application.DTO
{
    public class WateringEventResponseDto
    {
        public required int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int PlantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
