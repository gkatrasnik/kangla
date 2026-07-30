namespace kangla.Application.WateringCommands
{
    public class DeviceCheckInResponseDto
    {
        public DateTime ServerTimeUtc { get; set; }
        public DeviceWateringCommandDto? Command { get; set; }
    }

    public class DeviceWateringCommandDto
    {
        public int Id { get; set; }
        public int DurationSeconds { get; set; }
    }
}
