namespace kangla.Application.Plants.DTO
{
    public class PlantRecognizeResponseDto
    {
        public string? CommonName { get; set; }
        public string? LatinName { get; set; }
        public string? Description { get; set; }
        public string? AdditionalTips { get; set; }
        public int? WateringInterval { get; set; }
        public string? WateringInstructions { get; set; }
        public Guid? ImageId { get; set; }
        public string? Error { get; set; }
    }
}
