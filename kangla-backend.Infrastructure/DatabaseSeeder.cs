
using Microsoft.Extensions.Logging;
using Domain.Model;

namespace Infrastructure
{
    public  class DatabaseSeeder
    {
        private readonly WateringContext _context;
        private readonly JsonFileLoader _jsonFileLoader;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(WateringContext context, JsonFileLoader jsonFileLoader, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _jsonFileLoader = jsonFileLoader;
            _logger = logger;
        }

        public void Seed()
        {
            if (!_context.WateringDevices.Any())
            {
                var wateringDevices = _jsonFileLoader.LoadJson<List<WateringDevice>>("SeedData/watering_devices_seed_data.json");
                _context.WateringDevices.AddRange(wateringDevices);
                _context.SaveChanges();
            }

            if (!_context.WateringEvents.Any())
            {
                var wateringEvents = _jsonFileLoader.LoadJson<List<WateringEvent>>("SeedData/watering_events_seed_data.json");
                _context.WateringEvents.AddRange(wateringEvents);
                _context.SaveChanges();
            }

            if (!_context.HumidityMeasurements.Any())
            {
                var humidityMeasurements = _jsonFileLoader.LoadJson<List<HumidityMeasurement>>("SeedData/humidity_measurements_seed_data.json");
                _context.HumidityMeasurements.AddRange(humidityMeasurements);
                _context.SaveChanges();
            }

            _logger.LogInformation("Database seeded with jsondata.");

        }
    }
}
