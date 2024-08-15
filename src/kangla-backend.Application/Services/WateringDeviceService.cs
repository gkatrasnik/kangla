using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class WateringDeviceService : IWateringDeviceService
    {
        private readonly IWateringDeviceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public WateringDeviceService(IWateringDeviceRepository repository, IMapper mapper, IImageService imageService)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
        {            
            var wateringDevices = await _repository.GetWateringDevicesAsync(userId, pageNumber, pageSize);

            var wateringDeviceDtos = wateringDevices.Data.Select(device =>
            {
                var deviceDto = _mapper.Map<WateringDeviceResponseDto>(device);

                if (device.ImageData != null && device.ImageData.Length > 0)
                {
                    deviceDto.ImageBase64 = Convert.ToBase64String(device.ImageData);
                }

                return deviceDto;
            }).ToList();

            var pagedResponse = new PagedResponseDto<WateringDeviceResponseDto>
            {
                Data = wateringDeviceDtos,
                TotalRecords = wateringDevices.TotalRecords,
                TotalPages = wateringDevices.TotalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return pagedResponse;
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int deviceId, string userId)
        {            
            var wateringDevice = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with ID {deviceId} not found for current user.");
            var wateringDeviceDto = _mapper.Map<WateringDeviceResponseDto>(wateringDevice);

            if (wateringDevice.ImageData != null && wateringDevice.ImageData.Length > 0)
            {
                wateringDeviceDto.ImageBase64 = Convert.ToBase64String(wateringDevice.ImageData);
            }

            return wateringDeviceDto;
        }

        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice, string userId)
        {
            if (await _repository.WateringDeviceTokenExistsAsync(wateringDevice.DeviceToken))
            {
                throw new ArgumentException($"A device with device token {wateringDevice.DeviceToken} already exists.");
            }

            var entity = _mapper.Map<WateringDevice>(wateringDevice);
            entity.UserId = userId;

            if (wateringDevice.Image != null && wateringDevice.Image.Length > 0)
            {
                entity.ImageData = await _imageService.ProcessImageAsync(wateringDevice.Image, 300, 200, 50);
            }

            await _repository.AddWateringDeviceAsync(entity);

            var wateringDeviceDto = _mapper.Map<WateringDeviceResponseDto>(entity);
            if (entity.ImageData != null && entity.ImageData.Length > 0)
            {
                wateringDeviceDto.ImageBase64 = Convert.ToBase64String(entity.ImageData);
            }

            return wateringDeviceDto;
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");
            _mapper.Map(wateringDevice, existingEntity);

            if (wateringDevice.removeImage == true)
            {
                existingEntity.ImageData = null;
            }
            else if (wateringDevice.Image != null && wateringDevice.Image.Length > 0)
            {
                existingEntity.ImageData = await _imageService.ProcessImageAsync(wateringDevice.Image, 300, 200, 50);
            }

            await _repository.UpdateWateringDeviceAsync(existingEntity, userId);

            var wateringDeviceDto = _mapper.Map<WateringDeviceResponseDto>(existingEntity);
            if (existingEntity.ImageData != null && existingEntity.ImageData.Length > 0)
            {
                wateringDeviceDto.ImageBase64 = Convert.ToBase64String(existingEntity.ImageData);
            }

            return wateringDeviceDto;
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
