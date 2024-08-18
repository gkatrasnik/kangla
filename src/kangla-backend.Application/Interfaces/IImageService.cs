using Application.DTO;
using Domain.Entities;

namespace Application.Interfaces
{   
    public interface IImageService
    {
        Task<ImageResponseDto> GetImageAsync(int ImageId);
        Task<Image> CreateImageAsync(Image image);
        Task DeleteImageAsync(int imageId);
    }
}
