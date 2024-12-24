using kangla.Domain.Entities;

namespace kangla.Application.Interfaces
{
    public interface IImageService
    {
        Task<Image> GetImageAsync(int ImageId);
        Task<Image> CreateImageAsync(Image image);
        Task<bool> DeleteImageAsync(int imageId);
        string GenerateETag(byte[] imageData);
    }
}
