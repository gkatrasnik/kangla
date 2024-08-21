namespace Application.DTO
{
    public class PlantRecognizeResponseDto
    {
        public string Name { get; set; } = default!;
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public int? WateringInterval { get; set; }
        public string? WateringInstructions { get; set; }
        public int? ImageId { get; set; }
        public string? Error { get; set; }
    }
}
