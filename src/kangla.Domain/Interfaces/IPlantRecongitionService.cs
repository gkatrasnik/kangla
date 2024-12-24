using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IPlantRecognitionService
    {
        Task<PlantRecognitionResponse> RecognizePlantAsync(byte[] imageData);
    }
}
