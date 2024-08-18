using Microsoft.AspNetCore.Http;

public interface IImageProcessingService
{
    Task<byte[]> ProcessImageAsync(IFormFile image, int width, int height, int quality);
}