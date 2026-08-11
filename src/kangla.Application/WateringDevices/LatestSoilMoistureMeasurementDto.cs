namespace kangla.Application.WateringDevices
{
    public class LatestSoilMoistureMeasurementDto
    {
        public int RawSoilMoisture { get; set; }
        public int SoilMoisturePercentage { get; set; }
        public DateTime MeasuredAtUtc { get; set; }
    }
}
