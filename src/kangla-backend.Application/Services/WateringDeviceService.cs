using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class WateringDeviceService : IWateringDeviceService
    {
        private readonly IWateringDeviceRepository _wateringDeviceRepository;
        private readonly IPlantsRepository _plantsRepository;
        private readonly IMapper _mapper;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageService _imageService;

        public WateringDeviceService(IWateringDeviceRepository wateringDeviceRepository, IPlantsRepository plantsRepository,IMapper mapper, IImageProcessingService imageProcessingService, IImageService imageService)
        {
            _wateringDeviceRepository = wateringDeviceRepository;
            _plantsRepository = plantsRepository;
            _mapper = mapper;
            _imageProcessingService = imageProcessingService;
            _imageService = imageService;
        }

        public async Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
        {            
            var wateringDevices = await _wateringDeviceRepository.GetWateringDevicesAsync(userId, pageNumber, pageSize);
            return _mapper.Map<PagedResponseDto<WateringDeviceResponseDto>>(wateringDevices);           
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int deviceId, string userId)
        {            
            var wateringDevice = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with ID {deviceId} not found for current user.");
            return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceByPlantIdAsync(int plantId, string userId)
        {
            var wateringDevice = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Watering device for plant with ID {plantId} not found for current user.");
            return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDeviceDto, string userId)
        {
            if (await _wateringDeviceRepository.WateringDeviceTokenExistsAsync(wateringDeviceDto.DeviceToken))
            {
                throw new ArgumentException($"A device with device token {wateringDeviceDto.DeviceToken} already exists.");
            }

            var plant = await _plantsRepository.GetPlantByIdAsync(wateringDeviceDto.PlantId, userId);
            if (plant == null)
            {
                throw new ArgumentException($"No plant found with id {wateringDeviceDto.PlantId} for current user.");
            }           

            var existingDeviceForPlant = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(wateringDeviceDto.PlantId, userId);
            if (existingDeviceForPlant != null)
            {
                throw new InvalidOperationException($"The plant with id {wateringDeviceDto.PlantId} already has a watering device.");
            }

            var entity = _mapper.Map<WateringDevice>(wateringDeviceDto);
            entity.UserId = userId;

            await _wateringDeviceRepository.AddWateringDeviceAsync(entity);

            return _mapper.Map<WateringDeviceResponseDto>(entity);
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDeviceDto)
        {
            var existingEntity = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId)
                ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");

            var plantExists = await _plantsRepository.PlantExistsAsync(wateringDeviceDto.PlantId);
            if (!plantExists)
            {
                throw new ArgumentException($"The plant with ID {wateringDeviceDto.PlantId} does not exist.");
            }

            if (existingEntity.PlantId == wateringDeviceDto.PlantId)
            {
                throw new ArgumentException($"The plant with ID {wateringDeviceDto.PlantId} is already assigned to this watering device.");
            }

            var existingDeviceForPlant = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(wateringDeviceDto.PlantId, userId);
            if (existingDeviceForPlant != null)
            {
                throw new InvalidOperationException($"The plant with ID {wateringDeviceDto.PlantId} already has a different watering device.");
            }            

            _mapper.Map(wateringDeviceDto, existingEntity);

            await _wateringDeviceRepository.UpdateWateringDeviceAsync(existingEntity, userId);

            return _mapper.Map<WateringDeviceResponseDto>(existingEntity);
        }


        public async Task<bool> DeleteWateringDeviceAsync(int deviceId, string userId)
        {            
            var entityExists= await _wateringDeviceRepository.WateringDeviceExistsForUserAsync(deviceId, userId);
            if (entityExists == false)
            {
                return false;
            }

            await _wateringDeviceRepository.DeleteWateringDeviceAsync(deviceId);
            return true;           
        }
    }
}
