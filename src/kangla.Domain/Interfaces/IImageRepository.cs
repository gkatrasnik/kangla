using kangla.Domain.Entities;

namespace kangla.Domain.Interfaces
{
    public interface IImageRepository
    {
        Task<MediaImage?> GetImageAsync(int imageId);
        Task<MediaImage> AddImageAsync(MediaImage image);
        Task DeleteImageAsync(int imageId);
        Task<bool> ImageExistsAsync(int imageId);
        Task<string?> GetImageETagAsync(int imageId);
    }
}