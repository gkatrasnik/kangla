using kangla.Domain.Entities;

namespace kangla.Domain.Interfaces
{
    public interface IImageRepository
    {
        Task<MediaImage?> GetImageAsync(Guid imageId);
        Task<MediaImage> AddImageAsync(MediaImage image);
        Task DeleteImageAsync(Guid imageId);
        Task<bool> ImageExistsAsync(Guid imageId);
        Task<string?> GetImageETagAsync(Guid imageId);
    }
}