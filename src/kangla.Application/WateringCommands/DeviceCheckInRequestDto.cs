using System.ComponentModel.DataAnnotations;

namespace kangla.Application.WateringCommands
{
    public class DeviceCheckInRequestDto : IValidatableObject
    {
        [Range(0, 4095, ErrorMessage = "Raw soil moisture must be between 0 and 4095.")]
        public int? RawSoilMoisture { get; set; }

        [Range(0, 100, ErrorMessage = "Soil moisture percentage must be between 0 and 100.")]
        public int? SoilMoisturePercentage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RawSoilMoisture.HasValue != SoilMoisturePercentage.HasValue)
            {
                yield return new ValidationResult(
                    "Raw soil moisture and soil moisture percentage must be supplied together.",
                    new[] { nameof(RawSoilMoisture), nameof(SoilMoisturePercentage) });
            }
        }
    }
}
