using AutoMapper;
using kangla.Domain.Entities;
using kangla.Domain.Model;
using kangla.Application.Plants.DTO;
using kangla.Application.WateringEvents;
using kangla.Application.WateringDevices;
using kangla.Application.HumidityMeasurements;

namespace kangla.Application.Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WateringDevice, WateringDeviceResponseDto>();
            CreateMap<WateringDeviceCreateRequestDto, WateringDevice>();
            CreateMap<WateringDeviceUpdateRequestDto, WateringDevice>();

            CreateMap<Plant, PlantResponseDto>();
            CreateMap<PlantCreateRequestDto, Plant>();
            CreateMap<PlantUpdateRequestDto, Plant>();

            CreateMap<WateringEvent, WateringEventResponseDto>();
            CreateMap<WateringEventCreateRequestDto, WateringEvent>();

            CreateMap<HumidityMeasurement, HumidityMeasurementResponseDto>();
            CreateMap<HumidityMeasurementCreateRequestDto, HumidityMeasurement>();

            CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
        }
    }
}
