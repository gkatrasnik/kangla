using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace kangla_backend.Model
{

    public class WateringContext : DbContext
    {
        public WateringContext(DbContextOptions<WateringContext> options)
        : base(options)
        {
        }

        public DbSet<WateringDevice> WateringDevices { get; set; }
        public DbSet<WateringEvent> WateringEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            

            //TODO seed db - only for dev
            var wateringDevices = GetWateringDevicesFromJson("App_data/watering_devices_seed_data.json");
            modelBuilder.Entity<WateringDevice>().HasData(wateringDevices);

            var wateringEvents = GetWateringEventsFromJson("App_data/watering_events_seed_data.json");
            modelBuilder.Entity<WateringEvent>().HasData(wateringEvents);
        }

        private List<WateringDevice> GetWateringDevicesFromJson(string filePath)
        { 
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<WateringDevice>>(json);
        }

        private List<WateringEvent> GetWateringEventsFromJson(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<WateringEvent>>(json);
        }
    }
}
