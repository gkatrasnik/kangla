using kangla.Domain.Entities;

namespace kangla.Application.Images
{
    public interface IImageService
    {
        Task<MediaImage> GetImageAsync(Guid imageId, string userId);
        Task<MediaImage> CreateImageAsync(MediaImage image, string userId);
        Task<bool> DeleteImageAsync(Guid imageId, string userId);
        Task<string?> GetImageETagAsync(Guid imageId, string userId);
        string GenerateETag(byte[] imageData);
    }
}
