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


            //todo do we have imageId here?
            await _plantsRepository.AddPlantAsync(plantEntity);

            return _mapper.Map<PlantResponseDto>(plantEntity);
        }

        public async Task<PlantResponseDto> UpdatePlantAsync(int plantId, string userId, PlantUpdateRequestDto plantDto)
        {
            var existingEntity = await _plantsRepository.GetPlantByIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Plant with id {plantId} not found for current user.");
            _mapper.Map(plantDto, existingEntity);

            //delete image if 
            if (plantDto.RemoveImage == true)
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
            var plantEntity = await _plantsRepository.GetPlantByIdAsync(plantId, userId);
            if (plantEntity == null)
            {
                return false;
            }
            if (plantEntity.ImageId.HasValue)
            { 
                await _imageService.DeleteImageAsync(plantEntity.ImageId.Value);
            }
            await _plantsRepository.DeletePlantAsync(plantId);
            return true;           
        }

        public async Task<PlantRecognizeResponseDto> RecognizePlantAsync(PlantRecognizeRequestDto plantRecognizeDto) 
        {
            if (plantRecognizeDto.Image == null || plantRecognizeDto.Image.Length == 0)
            {
                throw new ArgumentException("No image provided");
            }

            var resizedImage = await _imageProcessingService.ProcessImageAsync(plantRecognizeDto.Image, 512, 512, 80);
            var newImageEntity = await _imageService.CreateImageAsync(new Image { Data = resizedImage });

            if (newImageEntity == null)
            {
                throw new InvalidOperationException("Could not save image");
            }


            // Convert image to Base64 (needed for AI recognition)
            var imageBase64 = Convert.ToBase64String(newImageEntity.Data);

            // TODO: Implement AI plant recognition logic using imageBase64
            // Example: var recognizedPlant = await _plantRecognitionService.RecognizePlantAsync(imageBase64);

            // Return the recognized plant details or a default response for now
            return new PlantRecognizeResponseDto
            {
                Name = "Tomato Test",
                ScientificName = "Lycopersicon esculentum",
                WateringInterval = 7,
                ImageId = newImageEntity.Id,
            };
        }
    }
}
