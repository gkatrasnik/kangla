using System.ComponentModel.DataAnnotations;

namespace kangla.Application.WateringCommands
{
    public class DeviceCheckInRequestDto
    {
        [Range(0, 1000, ErrorMessage = "Soil humidity must be between 0 and 1000.")]
        public int? SoilHumidity { get; set; }
    }
}
