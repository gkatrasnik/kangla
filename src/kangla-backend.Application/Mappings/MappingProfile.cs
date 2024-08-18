using AutoMapper;
using Application.DTO;
using Domain.Model;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WateringDevice, WateringDeviceResponseDto>();
            CreateMap<WateringDeviceCreateRequestDto, WateringDevice>();
            CreateMap<WateringDeviceUpdateRequestDto, WateringDevice>();

            CreateMap<WateringEvent, WateringEventResponseDto>();
            CreateMap<WateringEventCreateRequestDto, WateringEvent>();

            CreateMap<HumidityMeasurement, HumidityMeasurementResponseDto>();
            CreateMap<HumidityMeasurementCreateRequestDto, HumidityMeasurement>();
            
            CreateMap<Image, ImageResponseDto>();
            CreateMap<ImageResponseDto, Image>();

            CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
        }
    }
}
