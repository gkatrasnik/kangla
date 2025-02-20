namespace kangla.Application.Plants.DTO
{
    public class PlantResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public int WateringInterval { get; set; } //days
        public string? WateringInstructions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? ImageId { get; set; }
        public DateTime? LastWateringDateTime { get; set; }
    }
}
