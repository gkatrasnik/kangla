using kangla.Domain.Entities;

namespace kangla.Application.Interfaces
{
    public interface IImageService
    {
        Task<MediaImage> GetImageAsync(Guid imageId);
        Task<MediaImage> CreateImageAsync(MediaImage image);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<string?> GetImageETagAsync(Guid imageId);
        string GenerateETag(byte[] imageData);
    }
}
