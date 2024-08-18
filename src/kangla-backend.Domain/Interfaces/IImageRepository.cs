using Domain.Entities;
public interface IImageRepository
{
    Task<Image?> GetImageAsync(int imageId);
    Task<Image> AddImageAsync(Image image);
    Task DeleteImageAsync(int imageId);
    Task<bool> ImageExistsAsync(int imageId);
}