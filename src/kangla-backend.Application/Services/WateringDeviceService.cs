using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class WateringDeviceService : IWateringDeviceService
    {
        private readonly IWateringDeviceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageService _imageService;

        public WateringDeviceService(IWateringDeviceRepository repository, IMapper mapper, IImageProcessingService imageProcessingService, IImageService imageService)
        {
            _repository = repository;
            _mapper = mapper;
            _imageProcessingService = imageProcessingService;
            _imageService = imageService;
        }

        public async Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
        {            
            var wateringDevices = await _repository.GetWateringDevicesAsync(userId, pageNumber, pageSize);
            return _mapper.Map<PagedResponseDto<WateringDeviceResponseDto>>(wateringDevices);           
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int deviceId, string userId)
        {            
            var wateringDevice = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with ID {deviceId} not found for current user.");
            return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceByPlantIdAsync(int plantId, string userId)
        {
            var wateringDevice = await _repository.GetWateringDeviceByPlantIdAsync(plantId, userId) ?? throw new KeyNotFoundException($"Watering device for plant with ID {plantId} not found for current user.");
            return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDeviceDto, string userId)
        {
            if (await _repository.WateringDeviceTokenExistsAsync(wateringDeviceDto.DeviceToken))
            {
                throw new ArgumentException($"A device with device token {wateringDeviceDto.DeviceToken} already exists.");
            }

            var entity = _mapper.Map<WateringDevice>(wateringDeviceDto);
            entity.UserId = userId;

            await _repository.AddWateringDeviceAsync(entity);

            return _mapper.Map<WateringDeviceResponseDto>(entity);
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDeviceDto)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");
            _mapper.Map(wateringDeviceDto, existingEntity);

            await _repository.UpdateWateringDeviceAsync(existingEntity, userId);

            return _mapper.Map<WateringDeviceResponseDto>(existingEntity);
        }

        public async Task<bool> DeleteWateringDeviceAsync(int deviceId, string userId)
        {            
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(deviceId, userId);
            if (existingEntity == null)
            {
                return false;
            }

            await _repository.DeleteWateringDeviceAsync(deviceId);
            return true;           
        }
    }
}
