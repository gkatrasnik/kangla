using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class PlantsService : IPlantsService
    {
        private readonly IPlantsRepository _plantsRepository;
        private readonly IMapper _mapper;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageService _imageService;

        public PlantsService(IPlantsRepository plantsRepository, IMapper mapper, IImageProcessingService imageProcessingService, IImageService imageService)
        {
            _plantsRepository = plantsRepository;
            _mapper = mapper;
            _imageProcessingService = imageProcessingService;
            _imageService = imageService;
        }

        public async Task<PagedResponseDto<PlantResponseDto>> GetPlantsAsync(string userId, int pageNumber, int pageSize)
        {            
            var plants = await _plantsRepository.GetPlantsAsync(userId, pageNumber, pageSize);
            return _mapper.Map<PagedResponseDto<PlantResponseDto>>(plants);           
        }

        public async Task<PlantResponseDto> GetPlantAsync(int plantId, string userId)
        {            
            var plant = await _plantsRepository.GetPlantByIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Plant with ID {plantId} not found for current user.");
            return _mapper.Map<PlantResponseDto>(plant);
        }

        public async Task<PlantResponseDto> CreatePlantAsync(PlantCreateRequestDto plantDto, string userId)
        {
            var plantEntity = _mapper.Map<Plant>(plantDto);
            plantEntity.UserId = userId;


            // Todo this should be processed in transaction with adding plant to db
            if (plantDto.Image != null && plantDto.Image.Length > 0)
            {
               var resizedImage = await _imageProcessingService.ProcessImageAsync(plantDto.Image, 512, 512, 80);
                var newImage = new Image
                {
                    Data = resizedImage
                };
                newImage = await _imageService.CreateImageAsync(newImage);
                if (newImage?.Id != null) 
                {
                    plantEntity.ImageId = newImage.Id;
                }
            }

            await _plantsRepository.AddPlantAsync(plantEntity);

            return _mapper.Map<PlantResponseDto>(plantEntity);
        }

        public async Task<PlantResponseDto> UpdatePlantAsync(int plantId, string userId, PlantUpdateRequestDto plantDto)
        {
            var existingEntity = await _plantsRepository.GetPlantByIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Plant with id {plantId} not found for current user.");
            _mapper.Map(plantDto, existingEntity);

            //delete image if 
            if (plantDto.removeImage == true)
            {
                if (existingEntity.ImageId.HasValue) 
                {
                    await _imageService.DeleteImageAsync(existingEntity.ImageId.Value);
                }
                existingEntity.ImageId = null;
            }
            else if (plantDto.Image != null && plantDto.Image.Length > 0)
            {
                // if image was sent with request, create new image 
                var resizedImage = await _imageProcessingService.ProcessImageAsync(plantDto.Image, 512, 512, 80);
                var newImage = new Image
                {
                    Data = resizedImage
                };
                newImage = await _imageService.CreateImageAsync(newImage);

                //delete old image 
                if (existingEntity.ImageId.HasValue)
                {
                    await _imageService.DeleteImageAsync(existingEntity.ImageId.Value);
                }

                // assing new images id to wateringDevice
                if (newImage?.Id != null)
                {
                    existingEntity.ImageId = newImage.Id;
                }
            }

            await _plantsRepository.UpdatePlantAsync(existingEntity, userId);

            return _mapper.Map<PlantResponseDto>(existingEntity);
        }

        public async Task<bool> DeletePlantAsync(int plantId, string userId)
        {            
            var existingEntity = await _plantsRepository.GetPlantByIdAsync(plantId, userId);
            if (existingEntity == null)
            {
                return false;
            }

            await _plantsRepository.DeletePlantAsync(plantId);
            return true;           
        }
    }
}
