
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;

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
            if (_context.Plants.Any())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            var demoUser1 = new IdentityUser { UserName = "gkatrasnik@gmail.com", Email = "gkatrasnik@gmail.com" };
            var demoUser2 = new IdentityUser { UserName = "gkatrasnik+1@gmail.com", Email = "gkatrasnik+1@gmail.com" };

            var result1 = await _userManager.CreateAsync(demoUser1, "geslo123");
            var result2 = await _userManager.CreateAsync(demoUser2, "geslo123");

            if (result1.Succeeded && result2.Succeeded)
            {
                var confirmationToken1 = await _userManager.GenerateEmailConfirmationTokenAsync(demoUser1);
                var confirmationToken2 = await _userManager.GenerateEmailConfirmationTokenAsync(demoUser2);

                var confirmResult1 = await _userManager.ConfirmEmailAsync(demoUser1, confirmationToken1);
                var confirmResult2 = await _userManager.ConfirmEmailAsync(demoUser2, confirmationToken2);
            }
            else
            {
                _logger.LogError("Error creating demo users");
            }

            var demoUser1Id = demoUser1.Id;
            var demoUser2Id = demoUser2.Id;

            var plants = new List<Plant>
            {
                new Plant
                {
                    Name = "Rose",
                    ScientificName = "Rosa",
                    Description = "A red rose plant",
                    Location = "Garden",
                    Notes = "Needs regular pruning",
                    WateringInterval = 7,
                    WateringInstructions = "Water weekly during the growing season.",
                    UserId = demoUser1Id,
                    WateringDevice = new WateringDevice
                    {
                        WaterNow = false,
                        MinimumSoilHumidity = 400,
                        WateringIntervalSetting = 7,
                        WateringDurationSetting = 5,
                        DeviceToken = "device12345",
                        UserId = demoUser1Id,
                        HumidityMeasurements = new List<HumidityMeasurement>
                        {
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 657 },
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 349 }
                        }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Start = DateTime.Parse("2024-07-06T08:00:00Z"), End = DateTime.Parse("2024-07-06T08:05:00Z") }
                    }
                },
                new Plant
                {
                    Name = "Tomato",
                    ScientificName = "Solanum lycopersicum",
                    Description = "A tomato plant in the greenhouse",
                    Location = "Greenhouse",
                    Notes = "Check soil moisture frequently",
                    WateringInterval = 3,
                    WateringInstructions = "Water every 3 days during fruiting.",
                    UserId = demoUser1Id,
                    WateringDevice = new WateringDevice
                    {
                        WaterNow = true,
                        MinimumSoilHumidity = 450,
                        WateringIntervalSetting = 3,
                        WateringDurationSetting = 4,
                        DeviceToken = "device1236",
                        UserId = demoUser1Id,
                        HumidityMeasurements = new List<HumidityMeasurement>
                        {
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 362 },
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 693 }
                        }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Start = DateTime.Parse("2024-07-06T09:00:00Z"), End = DateTime.Parse("2024-07-06T09:04:00Z") }
                    }
                },
                new Plant
                {
                    Name = "Lavender",
                    ScientificName = "Lavandula",
                    Description = "Lavender in the front yard",
                    Location = "Front Yard",
                    Notes = "Attracts bees",
                    WateringInterval = 14,
                    WateringInstructions = "Water biweekly, less in winter.",
                    UserId = demoUser2Id,
                    WateringDevice = new WateringDevice
                    {
                        WaterNow = false,
                        MinimumSoilHumidity = 350,
                        WateringIntervalSetting = 14,
                        WateringDurationSetting = 3,
                        DeviceToken = "device1237",
                        UserId = demoUser2Id,
                        HumidityMeasurements = new List<HumidityMeasurement>
                        {
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 731 },
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 371 }
                        }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Start = DateTime.Parse("2024-07-06T10:00:00Z"), End = DateTime.Parse("2024-07-06T10:03:00Z") }
                    }
                },
                new Plant
                {
                    Name = "Mint",
                    ScientificName = "Mentha",
                    Description = "Mint plant in the front yard",
                    Location = "Front Yard 2",
                    Notes = "Spreads quickly",
                    WateringInterval = 5,
                    WateringInstructions = "Water every 5 days.",
                    UserId = demoUser2Id,
                    WateringDevice = new WateringDevice
                    {
                        WaterNow = false,
                        MinimumSoilHumidity = 375,
                        WateringIntervalSetting = 5,
                        WateringDurationSetting = 3,
                        DeviceToken = "device1238",
                        UserId = demoUser2Id,
                        HumidityMeasurements = new List<HumidityMeasurement>
                        {
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T06:00:00Z"), SoilHumidity = 734 },
                            new HumidityMeasurement { DateTime = DateTime.Parse("2024-07-06T07:00:00Z"), SoilHumidity = 374 }
                        }
                    },
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Start = DateTime.Parse("2024-07-07T10:00:00Z"), End = DateTime.Parse("2024-07-07T10:03:00Z") }
                    }
                }
            };

            _context.Plants.AddRange(plants);
            await _context.SaveChangesAsync();
        }

    }
}
