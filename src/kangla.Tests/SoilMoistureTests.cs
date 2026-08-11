using System.ComponentModel.DataAnnotations;
using kangla.Application.Plants.DTO;
using kangla.Application.WateringCommands;
using kangla.DeviceSimulator;
using kangla.Domain.Entities;
using kangla.Infrastructure;
using kangla.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace kangla.Tests;

public class SoilMoistureTests
{
    [Theory]
    [InlineData(3200, 0)]
    [InlineData(1500, 100)]
    [InlineData(2350, 50)]
    [InlineData(4095, 0)]
    [InlineData(0, 100)]
    public void CalculatePercentage_MapsAndClampsCalibratedReadings(int rawValue, int expectedPercentage)
    {
        Assert.Equal(expectedPercentage, SoilMoistureCalibration.CalculatePercentage(rawValue));
    }

    [Fact]
    public void DeviceCheckInRequest_RequiresRawAndPercentageTogether()
    {
        var request = new DeviceCheckInRequestDto { RawSoilMoisture = 2350 };

        var results = Validate(request);

        Assert.Contains(results, result => result.ErrorMessage!.Contains("supplied together"));
    }

    [Theory]
    [InlineData(-1, 50)]
    [InlineData(4096, 50)]
    [InlineData(2350, -1)]
    [InlineData(2350, 101)]
    public void DeviceCheckInRequest_RejectsOutOfRangeTelemetry(int rawValue, int percentage)
    {
        var request = new DeviceCheckInRequestDto
        {
            RawSoilMoisture = rawValue,
            SoilMoisturePercentage = percentage
        };

        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void PlantCreateRequest_RequiresDesiredMoistureTarget()
    {
        var request = new PlantCreateRequestDto
        {
            Name = "Fern",
            WateringInterval = 7
        };

        Assert.Contains(Validate(request), result => result.MemberNames.Contains(nameof(request.DesiredSoilMoisturePercentage)));
    }

    [Fact]
    public async Task LatestMeasurements_ReturnNewestCalibratedReadingAndIgnoreLegacyRawOnlyRows()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PlantsContext>().UseSqlite(connection).Options;
        await using var context = new PlantsContext(options);
        await context.Database.EnsureCreatedAsync();

        var firstDevice = new WateringDevice
        {
            UserId = "user-1",
            Plant = new Plant { Name = "Fern", UserId = "user-1", WateringInterval = 7 }
        };
        var legacyDevice = new WateringDevice
        {
            UserId = "user-1",
            Plant = new Plant { Name = "Palm", UserId = "user-1", WateringInterval = 7 }
        };
        context.HumidityMeasurements.AddRange(
            new HumidityMeasurement
            {
                WateringDevice = firstDevice,
                DateTime = new DateTime(2026, 8, 11, 8, 0, 0, DateTimeKind.Utc),
                RawSoilMoisture = 2500,
                SoilMoisturePercentage = 41
            },
            new HumidityMeasurement
            {
                WateringDevice = firstDevice,
                DateTime = new DateTime(2026, 8, 11, 9, 0, 0, DateTimeKind.Utc),
                RawSoilMoisture = 2180,
                SoilMoisturePercentage = 60
            },
            new HumidityMeasurement
            {
                WateringDevice = legacyDevice,
                DateTime = new DateTime(2026, 8, 11, 10, 0, 0, DateTimeKind.Utc),
                RawSoilMoisture = 500
            });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new HumidityMeasurementRepository(context);
        var latest = await repository.GetLatestHumidityMeasurementsByDeviceIdsAsync(new[] { firstDevice.Id, legacyDevice.Id });

        Assert.Single(latest);
        Assert.Equal(60, latest[firstDevice.Id].SoilMoisturePercentage);
        Assert.False(latest.ContainsKey(legacyDevice.Id));
    }

    private static IReadOnlyCollection<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), results, true);
        return results;
    }
}
