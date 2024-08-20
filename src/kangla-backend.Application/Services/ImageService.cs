using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
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

        public async Task<ImageResponseDto> GetImageAsync(int imageId)
        {
            var image = await _imageRepository.GetImageAsync(imageId);
            if (image is null)
            {
                throw new KeyNotFoundException($"Image with ID {imageId} can not be found.");
            }
            var imageResponseDto = _mapper.Map<ImageResponseDto>(image);

            if (image.Data!= null && image.Data.Length > 0)
            {
                imageResponseDto.ImageBase64 = Convert.ToBase64String(image.Data);
            }

            return imageResponseDto;
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
    }
}
