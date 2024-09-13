using Application.DTO;
using Domain.Entities;

namespace Application.Interfaces
{   
    public interface IImageService
    {
        Task<Image> GetImageAsync(int ImageId);
        Task<Image> CreateImageAsync(Image image);
        Task<bool> DeleteImageAsync(int imageId);
        string GenerateETag(byte[] imageData);
    }
}
