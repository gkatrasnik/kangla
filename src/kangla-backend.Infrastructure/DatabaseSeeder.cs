
using Microsoft.Extensions.Logging;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure
{
    public  class DatabaseSeeder
    {
        private readonly WateringContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public DatabaseSeeder(WateringContext context, ILogger<DatabaseSeeder> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (_context.WateringDevices.Any())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            // Add demo users
            var demoUser1 = new IdentityUser { UserName = "user1@example.com", Email = "user1@example.com" };
            var demoUser2 = new IdentityUser { UserName = "user2@example.com", Email = "user2@example.com" };

            await _userManager.CreateAsync(demoUser1, "Password1.");
            await _userManager.CreateAsync(demoUser2, "Password2.");

            var demoUser1Id = demoUser1.Id;
            var demoUser2Id = demoUser2.Id;

            var wateringDevices = new List<WateringDevice>
            {
                new WateringDevice
                {
                    Id = 1,
                    Name = "Device 1",
                    Description = "First watering device",
                    Location = "Garden",
                    Notes = "Needs regular maintenance",
                    Active = true,
                    Deleted = false,
                    WaterNow = false,
                    WateringIntervalSetting = 30,
                    WateringDurationSetting = 5,
                    DeviceToken = "abcdefghi0",
                    UserId = demoUser1Id,
                    HumidityMeasurements = new List<HumidityMeasurement>
                    {
                        new HumidityMeasurement { Id = 1, DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 657, WateringDeviceId = 1 },
                        new HumidityMeasurement { Id = 2, DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 349, WateringDeviceId = 1 },
                        new HumidityMeasurement { Id = 3, DateTime = DateTime.Parse("2024-07-06T08:00:00Z"), SoilHumidity = 472, WateringDeviceId = 1 },
                        new HumidityMeasurement { Id = 4, DateTime = DateTime.Parse("2024-07-06T09:00:00Z"), SoilHumidity = 512, WateringDeviceId = 1 },
                        new HumidityMeasurement { Id = 5, DateTime = DateTime.Parse("2024-07-06T10:00:00Z"), SoilHumidity = 276, WateringDeviceId = 1 }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Id = 1, Start = DateTime.Parse("2024-07-06T08:00:00Z"), End = DateTime.Parse("2024-07-06T08:05:00Z"), WateringDeviceId = 1 },
                        new WateringEvent { Id = 2, Start = DateTime.Parse("2024-07-07T08:00:00Z"), End = DateTime.Parse("2024-07-07T08:05:00Z"), WateringDeviceId = 1 }
                    }
                },
                new WateringDevice
                {
                    Id = 2,
                    Name = "Device 2",
                    Description = "Second watering device",
                    Location = "Greenhouse",
                    Notes = "Check humidity levels",
                    Active = true,
                    Deleted = false,
                    WaterNow = true,
                    WateringIntervalSetting = 20,
                    WateringDurationSetting = 4,
                    DeviceToken = "abcdefghi1",
                    UserId = demoUser1Id,
                    HumidityMeasurements = new List<HumidityMeasurement>
                    {
                        new HumidityMeasurement { Id = 6, DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 362, WateringDeviceId = 2 },
                        new HumidityMeasurement { Id = 7, DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 693, WateringDeviceId = 2 },
                        new HumidityMeasurement { Id = 8, DateTime = DateTime.Parse("2024-07-06T08:00:00Z"), SoilHumidity = 448, WateringDeviceId = 2 },
                        new HumidityMeasurement { Id = 9, DateTime = DateTime.Parse("2024-07-06T09:00:00Z"), SoilHumidity = 275, WateringDeviceId = 2 },
                        new HumidityMeasurement { Id = 10, DateTime = DateTime.Parse("2024-07-06T10:00:00Z"), SoilHumidity = 508, WateringDeviceId = 2 }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Id = 3, Start = DateTime.Parse("2024-07-06T09:00:00Z"), End = DateTime.Parse("2024-07-06T09:04:00Z"), WateringDeviceId = 2 },
                        new WateringEvent { Id = 4, Start = DateTime.Parse("2024-07-07T09:00:00Z"), End = DateTime.Parse("2024-07-07T09:04:00Z"), WateringDeviceId = 2 }
                    }
                },
                new WateringDevice
                {
                    Id = 3,
                    Name = "Device 3",
                    Description = "Third watering device",
                    Location = "Front Yard",
                    Notes = "Monitor water usage",
                    Active = true,
                    Deleted = false,
                    WaterNow = false,
                    WateringIntervalSetting = 25,
                    WateringDurationSetting = 3,
                    DeviceToken = "abcdefghi2",
                    UserId = demoUser2Id,
                    HumidityMeasurements = new List<HumidityMeasurement>
                    {
                        new HumidityMeasurement { Id = 11, DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 731, WateringDeviceId = 3 },
                        new HumidityMeasurement { Id = 12, DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 371, WateringDeviceId = 3 },
                        new HumidityMeasurement { Id = 13, DateTime = DateTime.Parse("2024-07-06T08:00:00Z"), SoilHumidity = 689, WateringDeviceId = 3 },
                        new HumidityMeasurement { Id = 14, DateTime = DateTime.Parse("2024-07-06T09:00:00Z"), SoilHumidity = 381, WateringDeviceId = 3 },
                        new HumidityMeasurement { Id = 15, DateTime = DateTime.Parse("2024-07-06T10:00:00Z"), SoilHumidity = 585, WateringDeviceId = 3 }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Id = 5, Start = DateTime.Parse("2024-07-06T10:00:00Z"), End = DateTime.Parse("2024-07-06T10:03:00Z"), WateringDeviceId = 3 },
                        new WateringEvent { Id = 6, Start = DateTime.Parse("2024-07-07T10:00:00Z"), End = DateTime.Parse("2024-07-07T10:03:00Z"), WateringDeviceId = 3 }
                    }
                },
                new WateringDevice
                {
                    Id = 4,
                    Name = "Device 4",
                    Description = "Fourth watering device",
                    Location = "Front Yard 2",
                    Notes = "Monitor water usage 2",
                    Active = true,
                    Deleted = false,
                    WaterNow = false,
                    WateringIntervalSetting = 25,
                    WateringDurationSetting = 3,
                    DeviceToken = "abcdefghi4",
                    UserId = demoUser2Id,
                    HumidityMeasurements = new List<HumidityMeasurement>
                    {
                        new HumidityMeasurement { Id = 16, DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 734, WateringDeviceId = 4 },
                        new HumidityMeasurement { Id = 17, DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 374, WateringDeviceId = 4 },
                        new HumidityMeasurement { Id = 18, DateTime = DateTime.Parse("2024-07-06T08:00:00Z"), SoilHumidity = 684, WateringDeviceId = 4 },
                        new HumidityMeasurement { Id = 19, DateTime = DateTime.Parse("2024-07-06T09:00:00Z"), SoilHumidity = 384, WateringDeviceId = 4 },
                        new HumidityMeasurement { Id = 20, DateTime = DateTime.Parse("2024-07-06T10:00:00Z"), SoilHumidity = 584, WateringDeviceId = 4 }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Id = 7, Start = DateTime.Parse("2024-07-06T10:00:00Z"), End = DateTime.Parse("2024-07-06T10:03:00Z"), WateringDeviceId = 4 },
                        new WateringEvent { Id = 8, Start = DateTime.Parse("2024-07-07T10:00:00Z"), End = DateTime.Parse("2024-07-07T10:03:00Z"), WateringDeviceId = 4 }
                    }
                }
            };

            _context.WateringDevices.AddRange(wateringDevices);
            await _context.SaveChangesAsync();
        }
    }
}
