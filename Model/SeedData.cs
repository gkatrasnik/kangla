using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace kangla_backend.Model
{
    public static class SeedData
    {
        public static List<WateringDevice> GetWateringDevicesFromJson(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<WateringDevice>>(json);
        }

        public static List<WateringEvent> GetWateringEventsFromJson(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<WateringEvent>>(json);
        }

        public static List<HumidityMeasurement> GetHumidityMeasurementsFromJson(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<HumidityMeasurement>>(json);
        }
        public static void Seed(ModelBuilder modelBuilder, string wateringDevicesPath, string wateringEventsPath, string humidityMeasurementsPath)
        {
            var wateringDevices = GetWateringDevicesFromJson(wateringDevicesPath);
            modelBuilder.Entity<WateringDevice>().HasData(wateringDevices);

            var wateringEvents = GetWateringEventsFromJson(wateringEventsPath);
            modelBuilder.Entity<WateringEvent>().HasData(wateringEvents);

            var humidityMeasurements = GetHumidityMeasurementsFromJson(humidityMeasurementsPath);
            modelBuilder.Entity<HumidityMeasurement>().HasData(humidityMeasurements);
        }
    }
}
