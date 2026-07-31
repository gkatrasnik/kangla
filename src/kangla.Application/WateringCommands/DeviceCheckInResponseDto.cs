namespace kangla.Application.WateringCommands
{
    public class DeviceCheckInResponseDto
    {
        public DateTime ServerTimeUtc { get; set; }
        // Only user-requested manual watering commands are returned until automatic watering is implemented.
        public DeviceWateringCommandDto? Command { get; set; }
    }

    public class DeviceWateringCommandDto
    {
        public int Id { get; set; }
        public int DurationSeconds { get; set; }
    }
}
