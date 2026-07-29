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

        public async Task<MediaImage> GetImageAsync(Guid imageId, string userId)
        {
            var image = await _imageRepository.GetImageAsync(imageId, userId);
            if (image is null)
            {
                throw new KeyNotFoundException($"Image with ID {imageId} can not be found.");
            }

            return image;
        }

        public async Task<MediaImage> CreateImageAsync(MediaImage image, string userId)
        {
            image.UserId = userId;
            var newImage = await _imageRepository.AddImageAsync(image);
            return newImage;
        }

        public async Task<bool> DeleteImageAsync(Guid imageId, string userId)
        {
            return await _imageRepository.DeleteImageAsync(imageId, userId);
        }

        public async Task<string?> GetImageETagAsync(Guid imageId, string userId)
        {
            return await _imageRepository.GetImageETagAsync(imageId, userId);
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
