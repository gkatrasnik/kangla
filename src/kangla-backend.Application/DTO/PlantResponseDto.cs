namespace Application.DTO
{
    public class PlantResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public int WateringInterval { get; set; }
        public string? WateringInstructions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ImageId { get; set; }
    }
}
