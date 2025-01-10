using kangla.Domain.Entities;

namespace kangla.Application.Interfaces
{
    public interface IImageService
    {
        Task<MediaImage> GetImageAsync(int imageId);
        Task<MediaImage> CreateImageAsync(MediaImage image);
        Task<bool> DeleteImageAsync(int imageId);
        Task<string?> GetImageETagAsync(int imageId);
        string GenerateETag(byte[] imageData);
    }
}
