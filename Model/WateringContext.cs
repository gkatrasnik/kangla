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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var wateringDevices = GetWateringDevicesFromJson("App_data/seed_data.json");

            modelBuilder.Entity<WateringDevice>().HasData();
        }

        private List<WateringDevice> GetWateringDevicesFromJson(string filePath)
        { 
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<WateringDevice>>(json);
        }
    }
}
