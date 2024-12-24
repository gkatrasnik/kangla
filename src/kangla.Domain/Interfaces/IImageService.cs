using Microsoft.AspNetCore.Http;

namespace kangla.Domain.Interfaces
{
    public interface IImageProcessingService
    {
        Task<byte[]> ProcessImageAsync(IFormFile image, int width, int height, int quality);
    }
}