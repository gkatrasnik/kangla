using System.Security.Cryptography;
using AutoMapper;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;

namespace kangla.Application.Images
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<Image> GetImageAsync(int imageId)
        {
            var image = await _imageRepository.GetImageAsync(imageId);
            if (image is null)
            {
                throw new KeyNotFoundException($"Image with ID {imageId} can not be found.");
            }

            return image;
        }

        public async Task<Image> CreateImageAsync(Image image)
        {
            var newImage = await _imageRepository.AddImageAsync(image);
            return newImage;
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var imageExists = await _imageRepository.ImageExistsAsync(imageId);
            if (!imageExists)
            {
                return false;
            }

            await _imageRepository.DeleteImageAsync(imageId);
            return true;
        }

        public string GenerateETag(byte[] imageData)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(imageData);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
