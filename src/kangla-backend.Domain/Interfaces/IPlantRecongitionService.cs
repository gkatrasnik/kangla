using Domain.Model;

namespace Domain.Interfaces
{
    public interface IPlantRecognitionService
    {
        Task<PlantRecognitionResponse> RecognizePlantAsync(byte[] imageData);
    }
}
