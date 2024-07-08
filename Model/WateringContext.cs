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
        public DbSet<HumidityMeasurement> HumidityMeasurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureRelationships(modelBuilder);

            // Seed data - only for dev - extract to separate class
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var wateringDevicesPath = "SeedData/watering_devices_seed_data.json";
                var wateringEventsPath = "SeedData/watering_events_seed_data.json";
                var humidityMeasurementsPath = "SeedData/humidity_measurements_seed_data.json";
                SeedData.Seed(modelBuilder, wateringDevicesPath, wateringEventsPath, humidityMeasurementsPath);
            }            
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WateringDevice>()
                .HasMany(w => w.WateringEvents)
                .WithOne(w => w.WateringDevice)
                .HasForeignKey(w => w.WateringDeviceId);

            modelBuilder.Entity<WateringDevice>()
                .HasMany(w => w.HumidityMeasurement)
                .WithOne(h => h.WateringDevice)
                .HasForeignKey(h => h.WateringDeviceId);
        }        
    }
}
