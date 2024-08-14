using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

public class ImageService : IImageService
{
    /// <summary>
    /// Resizes and compresses an image using ImageSharp and returns it as a byte array
    /// </summary>
    /// <param name="image"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="quality"></param>
    /// <returns>The resized and compressed image as a byte array</returns>
    public async Task<byte[]> ProcessImageAsync(IFormFile image, int width, int height, int quality)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        using var img = await Image.LoadAsync(memoryStream);

        img.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));

        var encoder = new JpegEncoder
        {
            Quality = quality
        };

        using var outputMemoryStream = new MemoryStream();
        await img.SaveAsync(outputMemoryStream, encoder);

        return outputMemoryStream.ToArray();
    }
}