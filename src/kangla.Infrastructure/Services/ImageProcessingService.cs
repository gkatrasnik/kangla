using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using kangla.Domain.Interfaces;

namespace kangla.Infrastructure.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private const long MaxUploadBytes = 5 * 1024 * 1024;
        private const long MaxImagePixels = 16_000_000;

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
            if (image.Length <= 0 || image.Length > MaxUploadBytes)
            {
                throw new ArgumentException("Image files must be between 1 byte and 5 MB.");
            }

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var imageInfo = await Image.IdentifyAsync(memoryStream);
            if (imageInfo is null || (long)imageInfo.Width * imageInfo.Height > MaxImagePixels)
            {
                throw new ArgumentException("Image dimensions exceed the maximum supported size.");
            }

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
}
