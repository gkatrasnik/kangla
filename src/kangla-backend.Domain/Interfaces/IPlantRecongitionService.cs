using Domain.Model;

namespace Domain.Interfaces
{
    public interface IPlantRecognitionService
    {
        Task<PlantRecognitionResult> RecognizePlantAsync(byte[] imageData);
    }
}
