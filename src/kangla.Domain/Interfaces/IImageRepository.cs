using kangla.Domain.Entities;

namespace kangla.Domain.Interfaces
{
    public interface IImageRepository
    {
        Task<MediaImage?> GetImageAsync(Guid imageId, string userId);
        Task<MediaImage> AddImageAsync(MediaImage image);
        Task<bool> DeleteImageAsync(Guid imageId, string userId);
        Task<string?> GetImageETagAsync(Guid imageId, string userId);
    }
}
