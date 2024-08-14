using Microsoft.AspNetCore.Http;

public interface IImageService
{
    Task<byte[]> ProcessImageAsync(IFormFile image, int width, int height, int quality);
}