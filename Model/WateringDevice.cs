namespace kangla_backend.Model
{
    public class WateringDevice
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public double SoilHumidity { get; set; }
        public DateTime LastWatering { get; set; }
        public bool WaterNow { get; set; }
        public int WateringInterval { get; set; }
        public int WateringDuration { get; set; }
        public List<WateringEvent> WateringEvents { get; set; } 
    }
}
