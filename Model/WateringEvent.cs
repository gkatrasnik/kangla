namespace kangla_backend.Model
{
    public class WateringEvent
    {
        public required int Id  { get; set; }
        public int WateringDeviceId { get; set; }
        public WateringDevice WateringDevice { get; set; }
        public required DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public double SoilHumidity { get; set; }
    }
}
