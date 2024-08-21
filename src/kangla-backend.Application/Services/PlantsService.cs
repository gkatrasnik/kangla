using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class PlantsService : IPlantsService
    {
        private readonly IPlantsRepository _plantsRepository;
        private readonly IMapper _mapper;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageService _imageService;
        private readonly IPlantRecognitionService _plantRecognitionService;

        public PlantsService(
            IPlantsRepository plantsRepository, 
            IMapper mapper, 
            IImageProcessingService imageProcessingService, 
            IImageService imageService,
            IPlantRecognitionService plantRecognitionService)
        {
            _plantsRepository = plantsRepository;
            _mapper = mapper;
            _imageProcessingService = imageProcessingService;
            _imageService = imageService;
            _plantRecognitionService = plantRecognitionService;
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

            await _plantsRepository.AddPlantAsync(plantEntity);

            return _mapper.Map<PlantResponseDto>(plantEntity);
        }

        public async Task<PlantResponseDto> UpdatePlantAsync(int plantId, string userId, PlantUpdateRequestDto plantDto)
        {
            var existingEntity = await _plantsRepository.GetPlantByIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Plant with id {plantId} not found for current user.");
            _mapper.Map(plantDto, existingEntity);

            //TODO manipulating image and updating plan should be done in one transaction

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

                // adding new images id to wateringDevice
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
            

            var recognizedPlant = await _plantRecognitionService.RecognizePlantAsync(resizedImage);

            Image? newImageEntity = null;
            if (string.IsNullOrEmpty(recognizedPlant.Error))
            {
                newImageEntity = await _imageService.CreateImageAsync(new Image { Data = resizedImage });

                if (newImageEntity == null)
                {
                    throw new InvalidOperationException("Could not save image");

                }
            }

            //Todo make better PlantRecognizeResponseDto properties
            // Return the recognized plant details or a default response for now
            return new PlantRecognizeResponseDto
            {
                CommonName = recognizedPlant.CommonName,
                LatinName = recognizedPlant.LatinName,
                Description = recognizedPlant.Description,
                WateringInstructions = recognizedPlant.WateringInstructions,
                WateringInterval = recognizedPlant.WateringInterval,
                ImageId = newImageEntity?.Id,
                AdditionalTips = recognizedPlant.AdditionalTips,
                Error = recognizedPlant.Error
            };
        }
    }
}
