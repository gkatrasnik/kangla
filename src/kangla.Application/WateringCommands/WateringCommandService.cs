using kangla.Application.HumidityMeasurements;
using kangla.Application.WateringDevices;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;

namespace kangla.Application.WateringCommands
{
    /// <summary>
    /// Coordinates the manual device-watering lifecycle: request, device acknowledgement, and result.
    /// It also handles device check-ins, storing an optional raw <see cref="HumidityMeasurement"/>
    /// before returning a pending watering command.
    /// </summary>
    public class WateringCommandService : IWateringCommandService
    {
        private static readonly TimeSpan CommandLifetime = TimeSpan.FromMinutes(15);
        private readonly IWateringCommandRepository _wateringCommandRepository;
        private readonly IWateringDeviceRepository _wateringDeviceRepository;
        private readonly IHumidityMeasurementRepository _humidityMeasurementRepository;

        public WateringCommandService(
            IWateringCommandRepository wateringCommandRepository,
            IWateringDeviceRepository wateringDeviceRepository,
            IHumidityMeasurementRepository humidityMeasurementRepository)
        {
            _wateringCommandRepository = wateringCommandRepository;
            _wateringDeviceRepository = wateringDeviceRepository;
            _humidityMeasurementRepository = humidityMeasurementRepository;
        }

        /// <summary>
        /// Creates a command for the device, or returns its current active command to prevent duplicates.
        /// </summary>
        public async Task<(WateringCommandResponseDto Command, bool Created)> CreateForUserAsync(int deviceId, string userId)
        {
            var device = await _wateringDeviceRepository.GetWateringDeviceByIdAsync(deviceId, userId)
                ?? throw new KeyNotFoundException($"Watering device with ID {deviceId} was not found.");

            var nowUtc = DateTime.UtcNow;
            var activeCommand = await _wateringCommandRepository.GetActiveForDeviceAsync(deviceId, nowUtc);
            if (activeCommand is not null)
            {
                return (ToResponse(activeCommand), false);
            }

            var command = new WateringCommand
            {
                WateringDeviceId = device.Id,
                DurationSeconds = device.WateringDurationSetting,
                RequestedAtUtc = nowUtc,
                ExpiresAtUtc = nowUtc.Add(CommandLifetime)
            };

            var (storedCommand, created) = await _wateringCommandRepository.CreateOrGetActiveAsync(command, nowUtc);
            return (ToResponse(storedCommand), created);
        }

        public async Task<WateringCommandResponseDto> GetForUserAsync(int deviceId, int commandId, string userId)
        {
            var command = await _wateringCommandRepository.GetByIdForUserAsync(commandId, deviceId, userId)
                ?? throw new KeyNotFoundException($"Watering command with ID {commandId} was not found.");

            await ExpireIfNeededAsync(command);
            return ToResponse(command);
        }

        public async Task CancelForUserAsync(int deviceId, int commandId, string userId)
        {
            var command = await _wateringCommandRepository.GetByIdForUserAsync(commandId, deviceId, userId)
                ?? throw new KeyNotFoundException($"Watering command with ID {commandId} was not found.");

            await ExpireIfNeededAsync(command);
            if (command.Status != WateringCommandStatus.Pending)
            {
                throw new InvalidOperationException("Only a pending watering command can be cancelled.");
            }

            command.Status = WateringCommandStatus.Cancelled;
            await _wateringCommandRepository.UpdateAsync(command);
        }

        /// <summary>
        /// Authenticates a device check-in, persists the supplied raw humidity reading when present,
        /// and returns the pending command that the device should execute.
        /// </summary>
        public async Task<DeviceCheckInResponseDto> CheckInAsync(DeviceCheckInRequestDto request, string deviceCredential)
        {
            var device = await GetDeviceForCredentialAsync(deviceCredential);
            var nowUtc = DateTime.UtcNow;

            if (request.SoilHumidity.HasValue)
            {
                await _humidityMeasurementRepository.AddHumidityMeasurementAsync(new HumidityMeasurement
                {
                    DateTime = nowUtc,
                    SoilHumidity = request.SoilHumidity.Value,
                    WateringDeviceId = device.Id
                });
            }

            var activeCommand = await _wateringCommandRepository.GetActiveForDeviceAsync(device.Id, nowUtc);
            return new DeviceCheckInResponseDto
            {
                ServerTimeUtc = nowUtc,
                Command = activeCommand?.Status == WateringCommandStatus.Pending
                    ? new DeviceWateringCommandDto { Id = activeCommand.Id, DurationSeconds = activeCommand.DurationSeconds }
                    : null
            };
        }

        public async Task<WateringCommandResponseDto> AcknowledgeAsync(int commandId, string deviceCredential)
        {
            var device = await GetDeviceForCredentialAsync(deviceCredential);
            var command = await _wateringCommandRepository.GetByIdForDeviceAsync(commandId, device.Id)
                ?? throw new KeyNotFoundException($"Watering command with ID {commandId} was not found.");

            await ExpireIfNeededAsync(command);
            if (command.Status == WateringCommandStatus.Acknowledged)
            {
                return ToResponse(command);
            }

            if (command.Status != WateringCommandStatus.Pending)
            {
                throw new InvalidOperationException("Only a pending watering command can be acknowledged.");
            }

            command.Status = WateringCommandStatus.Acknowledged;
            command.AcknowledgedAtUtc = DateTime.UtcNow;
            await _wateringCommandRepository.UpdateAsync(command);
            return ToResponse(command);
        }

        /// <summary>
        /// Records the device result. A completed result creates the linked plant watering event exactly once.
        /// </summary>
        public async Task<WateringCommandResponseDto> ReportResultAsync(int commandId, DeviceWateringCommandResultRequestDto request, string deviceCredential)
        {
            var device = await GetDeviceForCredentialAsync(deviceCredential);
            var command = await _wateringCommandRepository.GetByIdForDeviceAsync(commandId, device.Id)
                ?? throw new KeyNotFoundException($"Watering command with ID {commandId} was not found.");

            if (command.Status == WateringCommandStatus.Completed || command.Status == WateringCommandStatus.Failed)
            {
                return ToResponse(command);
            }

            if (command.Status != WateringCommandStatus.Acknowledged)
            {
                throw new InvalidOperationException("Only an acknowledged watering command can report a result.");
            }

            if (request.Outcome == WateringCommandOutcome.Completed)
            {
                if (!request.StartedAtUtc.HasValue || !request.FinishedAtUtc.HasValue || request.FinishedAtUtc < request.StartedAtUtc)
                {
                    throw new ArgumentException("Completed watering commands require ordered start and finish timestamps.");
                }

                command.Status = WateringCommandStatus.Completed;
                command.StartedAtUtc = request.StartedAtUtc.Value;
                command.FinishedAtUtc = request.FinishedAtUtc.Value;
                command.FailureReason = null;

                var wateringEvent = new WateringEvent
                {
                    PlantId = device.PlantId,
                    Start = request.StartedAtUtc.Value,
                    End = request.FinishedAtUtc.Value
                };

                await _wateringCommandRepository.CompleteAsync(command, wateringEvent);
                command.WateringEventId = wateringEvent.Id;
                return ToResponse(command);
            }

            if (request.Outcome == WateringCommandOutcome.Failed)
            {
                if (string.IsNullOrWhiteSpace(request.FailureReason))
                {
                    throw new ArgumentException("Failed watering commands require a failure reason.");
                }

                command.Status = WateringCommandStatus.Failed;
                command.FailureReason = request.FailureReason;
                command.StartedAtUtc = request.StartedAtUtc;
                command.FinishedAtUtc = request.FinishedAtUtc;
                await _wateringCommandRepository.UpdateAsync(command);
                return ToResponse(command);
            }

            throw new ArgumentException("Watering command outcome is invalid.");
        }

        private async Task<WateringDevice> GetDeviceForCredentialAsync(string deviceCredential)
        {
            if (string.IsNullOrWhiteSpace(deviceCredential))
            {
                throw new UnauthorizedAccessException("A device credential is required.");
            }

            var credentialHash = WateringDeviceService.HashDeviceCredential(deviceCredential);
            return await _wateringDeviceRepository.GetWateringDeviceByCredentialHashAsync(credentialHash)
                ?? await _wateringDeviceRepository.GetLegacyWateringDeviceByTokenAsync(deviceCredential)
                ?? throw new UnauthorizedAccessException("The device credential is invalid.");
        }

        private async Task ExpireIfNeededAsync(WateringCommand command)
        {
            if (command.Status == WateringCommandStatus.Pending && command.ExpiresAtUtc <= DateTime.UtcNow)
            {
                command.Status = WateringCommandStatus.Expired;
                await _wateringCommandRepository.UpdateAsync(command);
            }
        }

        private static WateringCommandResponseDto ToResponse(WateringCommand command)
        {
            return new WateringCommandResponseDto
            {
                Id = command.Id,
                DeviceId = command.WateringDeviceId,
                Status = command.Status,
                DurationSeconds = command.DurationSeconds,
                RequestedAtUtc = command.RequestedAtUtc,
                ExpiresAtUtc = command.ExpiresAtUtc,
                AcknowledgedAtUtc = command.AcknowledgedAtUtc,
                StartedAtUtc = command.StartedAtUtc,
                FinishedAtUtc = command.FinishedAtUtc,
                FailureReason = command.FailureReason,
                WateringEventId = command.WateringEventId
            };
        }
    }
}
