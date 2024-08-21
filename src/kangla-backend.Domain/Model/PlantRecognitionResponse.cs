
namespace Domain.Model
{
    public class PlantRecognitionResult
    {
        public string CommonName { get; set; } = default!;
        public string LatinName { get; set; } = default!;
        public string WateringInstructions { get; set; } = default!;
        public int WateringInterval { get; set; }
        public string AdditionalTips { get; set; } = default!;
        public string Error { get; set; } = default!;
    }
}
