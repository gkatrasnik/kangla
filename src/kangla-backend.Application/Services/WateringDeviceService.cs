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

        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice, string userId)
        {
            if (await _repository.WateringDeviceTokenExistsAsync(wateringDevice.DeviceToken))
            {
                throw new ArgumentException($"A device with device token {wateringDevice.DeviceToken} already exists.");
            }

            var entity = _mapper.Map<WateringDevice>(wateringDevice);
            entity.UserId = userId;


            // Todo this should be processed in transaction with adding watering device
            if (wateringDevice.Image != null && wateringDevice.Image.Length > 0)
            {
               var resizedImage = await _imageProcessingService.ProcessImageAsync(wateringDevice.Image, 512, 512, 80);
                var newImage = new Image
                {
                    Data = resizedImage
                };
                newImage = await _imageService.CreateImageAsync(newImage);
                if (newImage?.Id != null) 
                { 
                    entity.ImageId = newImage.Id;
                }
            }

            await _repository.AddWateringDeviceAsync(entity);

            return _mapper.Map<WateringDeviceResponseDto>(entity);
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");
            _mapper.Map(wateringDevice, existingEntity);

            //delete image if 
            if (wateringDevice.removeImage == true)
            {
                if (existingEntity.ImageId.HasValue) 
                {
                    await _imageService.DeleteImageAsync(existingEntity.ImageId.Value);
                }
                existingEntity.ImageId = null;
            }
            else if (wateringDevice.Image != null && wateringDevice.Image.Length > 0)
            {
                // if image was sent with request, create new image 
                var resizedImage = await _imageProcessingService.ProcessImageAsync(wateringDevice.Image, 512, 512, 80);
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
