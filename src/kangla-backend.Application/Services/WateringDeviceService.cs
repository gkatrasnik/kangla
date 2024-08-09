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

        public WateringDeviceService(IWateringDeviceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            await _repository.AddWateringDeviceAsync(entity);
            return _mapper.Map<WateringDeviceResponseDto>(entity);            
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");
            _mapper.Map(wateringDevice, existingEntity);
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
