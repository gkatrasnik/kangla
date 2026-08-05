using AutoMapper;
using kangla.Application.ClientUpdates;
using kangla.Application.Images;
using kangla.Application.Shared;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using System.Text;

namespace kangla.Application.WateringDevices
{
    public class WateringDeviceService : IWateringDeviceService
    {
        private readonly IWateringDeviceRepository _wateringDeviceRepository;
        private readonly IWateringCommandRepository _wateringCommandRepository;
        private readonly IPlantsRepository _plantsRepository;
        private readonly IMapper _mapper;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageService _imageService;
        private readonly IClientStateChangeNotifier _clientStateChangeNotifier;
        private readonly TimeProvider _timeProvider;

        public WateringDeviceService(
            IWateringDeviceRepository wateringDeviceRepository,
            IWateringCommandRepository wateringCommandRepository,
            IPlantsRepository plantsRepository,
            IMapper mapper,
            IImageProcessingService imageProcessingService,
            IImageService imageService,
            IClientStateChangeNotifier clientStateChangeNotifier,
            TimeProvider timeProvider)
        {
            _wateringDeviceRepository = wateringDeviceRepository;
            _wateringCommandRepository = wateringCommandRepository;
            _plantsRepository = plantsRepository;
            _mapper = mapper;
            _imageProcessingService = imageProcessingService;
            _imageService = imageService;
            _clientStateChangeNotifier = clientStateChangeNotifier;
            _timeProvider = timeProvider;
        }

        public async Task<PagedResponseDto<WateringDeviceResponseDto>> GetWateringDevicesAsync(string userId, int pageNumber, int pageSize)
        {
            var wateringDevices = await _wateringDeviceRepository.GetWateringDevicesAsync(userId, pageNumber, pageSize);
            var response = _mapper.Map<PagedResponseDto<WateringDeviceResponseDto>>(wateringDevices);
            var activeCommands = await _wateringCommandRepository.GetActiveForDevicesAsync(
                wateringDevices.Data.Select(device => device.Id).ToArray(),
                _timeProvider.GetUtcNow().UtcDateTime);
            var statusesByDeviceId = activeCommands.ToDictionary(command => command.WateringDeviceId, command => command.Status);
            foreach (var device in response.Data)
            {
                device.ActiveWateringCommandStatus = statusesByDeviceId.TryGetValue(device.Id, out var status)
                    ? status
                    : null;
            }

            return response;
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int deviceId, string userId)
        {
            var wateringDevice = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId) ?? throw new KeyNotFoundException($"Watering device with ID {deviceId} not found for current user.");
            return await ToResponseAsync(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto?> GetWateringDeviceByPlantIdAsync(int plantId, string userId)
        {
            var wateringDevice = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(plantId, userId);
            return wateringDevice is null ? null : await ToResponseAsync(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> ClaimWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDeviceDto, string userId)
        {
            if (wateringDeviceDto.PlantId.HasValue)
            {
                var plant = await _plantsRepository.GetPlantByIdAsync(wateringDeviceDto.PlantId.Value, userId);
                if (plant == null)
                {
                    throw new ArgumentException($"No plant found with id {wateringDeviceDto.PlantId} for current user.");
                }

                var existingDeviceForPlant = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(wateringDeviceDto.PlantId.Value, userId);
                if (existingDeviceForPlant != null)
                {
                    throw new InvalidOperationException($"The plant with id {wateringDeviceDto.PlantId} already has a watering device.");
                }
            }

            var entity = await _wateringDeviceRepository.GetUnclaimedWateringDeviceByAccessKeyHashAsync(HashDeviceAccessKey(wateringDeviceDto.DeviceAccessKey))
                ?? throw new ArgumentException("The device access key is invalid or the device has already been claimed.");

            _mapper.Map(wateringDeviceDto, entity);
            entity.UserId = userId;
            await _wateringDeviceRepository.ClaimWateringDeviceAsync(entity, userId);

            return _mapper.Map<WateringDeviceResponseDto>(entity);
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int deviceId, string userId, WateringDeviceUpdateRequestDto wateringDeviceDto)
        {
            var existingEntity = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId)
                ?? throw new KeyNotFoundException($"Watering device with id {deviceId} not found for current user.");

            if (wateringDeviceDto.PlantId.HasValue && existingEntity.PlantId != wateringDeviceDto.PlantId)
            {
                var plantExists = await _plantsRepository.PlantExistsForUserAsync(wateringDeviceDto.PlantId.Value, userId);
                if (!plantExists)
                {
                    throw new KeyNotFoundException($"The plant with ID {wateringDeviceDto.PlantId} was not found.");
                }

                var existingDeviceForPlant = await _wateringDeviceRepository.GetWateringDeviceByPlantIdAsync(wateringDeviceDto.PlantId.Value, userId);
                if (existingDeviceForPlant != null)
                {
                    throw new InvalidOperationException($"The plant with ID {wateringDeviceDto.PlantId} already has a different watering device.");
                }

                await CancelPendingCommandOrBlockInProgressCommandAsync(existingEntity);
            }

            existingEntity.MinimumSoilHumidity = wateringDeviceDto.MinimumSoilHumidity;
            existingEntity.WateringIntervalSetting = wateringDeviceDto.WateringIntervalSetting;
            existingEntity.WateringDurationSetting = wateringDeviceDto.WateringDurationSetting;
            if (wateringDeviceDto.PlantId.HasValue)
            {
                existingEntity.PlantId = wateringDeviceDto.PlantId.Value;
            }

            await _wateringDeviceRepository.UpdateWateringDeviceAsync(existingEntity, userId);

            return await ToResponseAsync(existingEntity);
        }


        public async Task<bool> DetachWateringDeviceAsync(int deviceId, string userId)
        {
            var entity = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId);
            if (entity is null)
            {
                return false;
            }

            await CancelPendingCommandOrBlockInProgressCommandAsync(entity);
            return await _wateringDeviceRepository.DetachWateringDeviceAsync(deviceId, userId);
        }

        public async Task<bool> DeleteWateringDeviceAsync(int deviceId, string userId)
        {
            var entity = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId);
            if (entity is null)
            {
                return false;
            }

            await CancelPendingCommandOrBlockInProgressCommandAsync(entity);
            return await _wateringDeviceRepository.DeleteWateringDeviceAsync(deviceId, userId);
        }

        private async Task CancelPendingCommandOrBlockInProgressCommandAsync(WateringDevice device)
        {
            var activeCommand = await _wateringCommandRepository.GetActiveForDeviceAsync(
                device.Id,
                _timeProvider.GetUtcNow().UtcDateTime);
            if (activeCommand is null)
            {
                return;
            }

            if (activeCommand.Status == WateringCommandStatus.Acknowledged)
            {
                throw new InvalidOperationException("The device cannot be detached, moved, or removed while it is watering. Wait for the command to complete or fail.");
            }

            var cancelled = await _wateringCommandRepository.TrySetStatusAsync(
                activeCommand.Id,
                WateringCommandStatus.Pending,
                WateringCommandStatus.Cancelled);
            if (!cancelled)
            {
                var currentCommand = await _wateringCommandRepository.GetByIdForDeviceAsync(activeCommand.Id, device.Id);
                if (currentCommand?.Status == WateringCommandStatus.Acknowledged)
                {
                    throw new InvalidOperationException("The device cannot be detached, moved, or removed while it is watering. Wait for the command to complete or fail.");
                }

                return;
            }

            if (device.UserId is not null)
            {
                await _clientStateChangeNotifier.NotifyAsync(device.UserId, new ClientStateChangedDto
                {
                    PlantId = device.PlantId,
                    DeviceId = device.Id,
                    Resources = new[] { ClientStateResource.WateringCommands },
                    OccurredAtUtc = _timeProvider.GetUtcNow().UtcDateTime
                });
            }
        }

        private async Task<WateringDeviceResponseDto> ToResponseAsync(WateringDevice device)
        {
            var response = _mapper.Map<WateringDeviceResponseDto>(device);
            var activeCommand = await _wateringCommandRepository.GetActiveForDeviceAsync(
                device.Id,
                _timeProvider.GetUtcNow().UtcDateTime);
            response.ActiveWateringCommandStatus = activeCommand?.Status;
            return response;
        }

        public static string HashDeviceAccessKey(string accessKey)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(accessKey)));
        }
    }
}
