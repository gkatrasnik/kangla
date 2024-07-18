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
            var wateringDevices = await _repository.GetWateringDevicesAsync();
            return _mapper.Map<IEnumerable<WateringDeviceResponseDto>>(wateringDevices);
        }

        public async Task<WateringDeviceResponseDto> GetWateringDeviceAsync(int id)
        {
            var wateringDevice = await _repository.GetWateringDeviceByIdAsync(id);
            return _mapper.Map<WateringDeviceResponseDto>(wateringDevice);
        }

        public async Task<WateringDeviceResponseDto> CreateWateringDeviceAsync(WateringDeviceCreateRequestDto wateringDevice)
        {
            var entity = _mapper.Map<WateringDevice>(wateringDevice);
            await _repository.AddWateringDeviceAsync(entity);
            return _mapper.Map<WateringDeviceResponseDto>(entity);
        }

        public async Task<WateringDeviceResponseDto> UpdateWateringDeviceAsync(int id, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(id) ?? throw new Exception($"Watering device with id {id} not found.");
            _mapper.Map(wateringDevice, existingEntity);

            await _repository.UpdateWateringDeviceAsync(existingEntity);

            return _mapper.Map<WateringDeviceResponseDto>(existingEntity);
        }

        public async Task<bool> DeleteWateringDeviceAsync(int id)
        {
            var existingEntity = await _repository.GetWateringDeviceByIdAsync(id);
            if (existingEntity == null)
            {
                return false;
            }

            await _repository.DeleteWateringDeviceAsync(id);
            return true;
        }

        public bool WateringDeviceExists(int id)
        {
            return _repository.WateringDeviceExists(id);
        }
    }
}
