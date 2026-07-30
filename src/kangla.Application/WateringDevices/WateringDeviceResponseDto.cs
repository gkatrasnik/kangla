namespace kangla.Application.WateringDevices
{
    public class WateringDeviceResponseDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int MinimumSoilHumidity { get; set; }
        public int WateringIntervalSetting { get; set; }
        public int WateringDurationSetting { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int PlantId { get; set; }
        /// <summary>
        /// Returned only when a credential is created or rotated. Store it on the physical device.
        /// </summary>
        public string? DeviceCredential { get; set; }
    }
}
