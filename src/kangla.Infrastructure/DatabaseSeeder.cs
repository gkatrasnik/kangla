
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using kangla.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace kangla.Infrastructure
{
    public class DatabaseSeeder
    {
        private readonly PlantsContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public DatabaseSeeder(PlantsContext context, ILogger<DatabaseSeeder> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await SeedSimulatorDeviceAsync();

            if (_context.Plants.Any())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            var demoUser1 = new IdentityUser { UserName = "demo.user@example.com", Email = "demo.user@example.com" };
            var demoUser2 = new IdentityUser { UserName = "demo.user2@example.com", Email = "demo.user2@example.com" };

            var result1 = await _userManager.CreateAsync(demoUser1, "DemoPassword123!");
            var result2 = await _userManager.CreateAsync(demoUser2, "DemoPassword123!");

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
                    WateringEvents = new List<WateringEvent>
                    {
                        new WateringEvent { Start = DateTime.Parse("2024-07-07T10:00:00Z"), End = DateTime.Parse("2024-07-07T10:03:00Z") }
                    }
                }
            };

            _context.Plants.AddRange(plants);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSimulatorDeviceAsync()
        {
            const string simulatorAccessKey = "kangla-simulator-01";
            var accessKeyHash = HashDeviceAccessKey(simulatorAccessKey);
            if (await _context.WateringDevices.AnyAsync(device => device.DeviceAccessKeyHash == accessKeyHash))
            {
                return;
            }

            _context.WateringDevices.Add(new WateringDevice
            {
                MinimumSoilHumidity = 400,
                WateringIntervalSetting = 7,
                WateringDurationSetting = 3,
                DeviceAccessKeyHash = accessKeyHash
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded the unclaimed development simulator device.");
        }

        private static string HashDeviceAccessKey(string accessKey)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accessKey)));
        }

    }
}
