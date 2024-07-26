using Application.DTO;
using Application.Interfaces;
using AutoMapper;
using Domain.Model;

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

        public async Task<IEnumerable<WateringDeviceResponseDto>> GetWateringDevicesAsync()
        {
            try
            {
                var wateringDevices = await _repository.GetWateringDevicesAsync();
                return _mapper.Map<IEnumerable<WateringDeviceResponseDto>>(wateringDevices);
            }
            catch (Exception ex)
            {
                // _log
                throw new Exception("An error occurred while retrieving watering devices.", ex);
            }
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int id)
        {
            try
            {
                var wateringDevice = await _repository.GetWateringDeviceByIdAsync(id);

                if (wateringDevice == null)
                {
                    throw new KeyNotFoundException($"Watering device with ID {id} not found.");
                }

                return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
            }
            catch (KeyNotFoundException)
            {
                // _logger.LogInformation($"Watering device with ID {id} not found.");                
                throw;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while retrieving the watering device.");
                throw new ApplicationException("An error occurred while retrieving the watering device.", ex);
            }
        }


        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice)
        {
            try
            {
                var entity = _mapper.Map<WateringDevice>(wateringDevice);
                await _repository.AddWateringDeviceAsync(entity);
                return _mapper.Map<WateringDeviceResponseDto>(entity);
            }
            catch (Exception ex)
            {
                // log error
                throw new ApplicationException("An error occurred while creating the watering device.", ex);
            }
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int id, WateringDeviceUpdateRequestDto wateringDevice)
        {
            try
            {
                var existingEntity = await _repository.GetWateringDeviceByIdAsync(id);

                if (existingEntity == null)
                {
                    throw new KeyNotFoundException($"Watering device with id {id} not found.");
                }

                _mapper.Map(wateringDevice, existingEntity);
                await _repository.UpdateWateringDeviceAsync(existingEntity);

                return _mapper.Map<WateringDeviceResponseDto>(existingEntity);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                // log
                throw new ArgumentException("Invalid data provided for updating the watering device.", ex);
            }
            catch (Exception ex)
            {
                // log
                throw new ApplicationException("An error occurred while updating the watering device.", ex);
            }
        }

        public async Task<bool> DeleteWateringDeviceAsync(int id)
        {
            try
            {
                var existingEntity = await _repository.GetWateringDeviceByIdAsync(id);
                if (existingEntity == null)
                {
                    return false;
                }

                await _repository.DeleteWateringDeviceAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"An error occurred while deleting the watering device with ID {id}.");
                throw new ApplicationException($"An error occurred while deleting the watering device with ID {id}.", ex);
            }
        }

        public bool WateringDeviceExists(int id)
        {
            return _repository.WateringDeviceExists(id);
        }
    }
}
