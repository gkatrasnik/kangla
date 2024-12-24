namespace kangla.Domain.Model
{
    public class PlantRecognitionResponse
    {
        public string? CommonName { get; set; }
        public string? LatinName { get; set; }
        public string? Description { get; set; }
        public string? WateringInstructions { get; set; }
        public int? WateringInterval { get; set; }
        public string? AdditionalTips { get; set; }
        public string? Error { get; set; }
    }
}
