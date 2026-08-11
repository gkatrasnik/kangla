using AutoMapper;
using kangla.Application.Shared;
using kangla.Application.WateringDevices;
using kangla.Domain.Entities;
using kangla.Infrastructure;
using kangla.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace kangla.Tests;

public class WateringDeviceServiceTests
{
    [Fact]
    public async Task GetWateringDevicesAsync_ReturnsUserScopedActiveCommandStatuses()
    {
        var nowUtc = new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero);
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PlantsContext>().UseSqlite(connection).Options;
        await using var context = new PlantsContext(options);
        await context.Database.EnsureCreatedAsync();

        var activeDevice = AddDevice(context, "user-1", WateringCommandStatus.Acknowledged, nowUtc.UtcDateTime);
        var completedDevice = AddDevice(context, "user-1", WateringCommandStatus.Completed, nowUtc.UtcDateTime);
        AddDevice(context, "user-2", WateringCommandStatus.Pending, nowUtc.UtcDateTime);
        context.HumidityMeasurements.Add(new HumidityMeasurement
        {
            WateringDevice = activeDevice,
            DateTime = nowUtc.UtcDateTime,
            RawSoilMoisture = 2350,
            SoilMoisturePercentage = 50
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var mapperConfiguration = new MapperConfiguration(configuration => configuration.AddProfile<MappingProfile>());
        var service = new WateringDeviceService(
            new WateringDeviceRepository(context),
            new WateringCommandRepository(context),
            new HumidityMeasurementRepository(context),
            null!,
            mapperConfiguration.CreateMapper(),
            null!,
            null!,
            null!,
            new FixedTimeProvider(nowUtc));

        var response = await service.GetWateringDevicesAsync("user-1", 1, 10);

        Assert.Equal(2, response.TotalRecords);
        Assert.Equal(
            WateringCommandStatus.Acknowledged,
            response.Data.Single(device => device.Id == activeDevice.Id).ActiveWateringCommandStatus);
        Assert.Equal(
            50,
            response.Data.Single(device => device.Id == activeDevice.Id).LatestSoilMoistureMeasurement?.SoilMoisturePercentage);
        Assert.Null(response.Data.Single(device => device.Id == completedDevice.Id).ActiveWateringCommandStatus);
    }

    private static WateringDevice AddDevice(
        PlantsContext context,
        string userId,
        WateringCommandStatus status,
        DateTime nowUtc)
    {
        var plant = new Plant { Name = $"{userId} {status}", UserId = userId, WateringInterval = 7 };
        var device = new WateringDevice { UserId = userId, Plant = plant, WateringDurationSetting = 3 };
        context.WateringCommands.Add(new WateringCommand
        {
            WateringDevice = device,
            Status = status,
            DurationSeconds = 3,
            RequestedAtUtc = nowUtc.AddMinutes(-1),
            ExpiresAtUtc = nowUtc.AddMinutes(14),
            AcknowledgedAtUtc = status == WateringCommandStatus.Acknowledged ? nowUtc.AddSeconds(-1) : null
        });
        return device;
    }

    private sealed class FixedTimeProvider(DateTimeOffset nowUtc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => nowUtc;
    }
}
