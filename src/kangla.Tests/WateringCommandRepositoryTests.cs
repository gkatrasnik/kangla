using kangla.Domain.Entities;
using kangla.Infrastructure;
using kangla.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace kangla.Tests;

public class WateringCommandRepositoryTests
{
    [Fact]
    public async Task GetActiveForDeviceAsync_TimesOutAnOverdueAcknowledgedCommand()
    {
        var acknowledgedAtUtc = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc);
        await using var fixture = await RepositoryFixture.CreateAsync(acknowledgedAtUtc);

        var activeCommand = await fixture.Repository.GetActiveForDeviceAsync(
            fixture.DeviceId,
            acknowledgedAtUtc.AddSeconds(3).AddMinutes(2));

        Assert.Null(activeCommand);
        Assert.Equal(WateringCommandStatus.TimedOut, await fixture.GetCommandStatusAsync());
    }

    [Fact]
    public async Task GetActiveForDeviceAsync_KeepsAcknowledgedCommandBeforeDeadline()
    {
        var acknowledgedAtUtc = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc);
        await using var fixture = await RepositoryFixture.CreateAsync(acknowledgedAtUtc);

        var activeCommand = await fixture.Repository.GetActiveForDeviceAsync(
            fixture.DeviceId,
            acknowledgedAtUtc.AddSeconds(3).AddMinutes(2).AddTicks(-1));

        Assert.NotNull(activeCommand);
        Assert.Equal(WateringCommandStatus.Acknowledged, activeCommand.Status);
    }

    [Fact]
    public async Task GetActiveForDevicesAsync_ReturnsOnlyActiveCommandsAndReconcilesExpiredPendingCommands()
    {
        var nowUtc = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc);
        await using var fixture = await RepositoryFixture.CreateAsync(nowUtc.AddMinutes(-1));
        var activePending = await fixture.AddCommandAsync(WateringCommandStatus.Pending, nowUtc.AddMinutes(10));
        var expiredPending = await fixture.AddCommandAsync(WateringCommandStatus.Pending, nowUtc.AddTicks(-1));
        var completed = await fixture.AddCommandAsync(WateringCommandStatus.Completed, nowUtc.AddMinutes(10));

        var activeCommands = await fixture.Repository.GetActiveForDevicesAsync(
            new[] { fixture.DeviceId, activePending.DeviceId, expiredPending.DeviceId, completed.DeviceId },
            nowUtc);

        Assert.Equal(2, activeCommands.Count);
        Assert.Contains(activeCommands, command => command.WateringDeviceId == fixture.DeviceId
            && command.Status == WateringCommandStatus.Acknowledged);
        Assert.Contains(activeCommands, command => command.WateringDeviceId == activePending.DeviceId
            && command.Status == WateringCommandStatus.Pending);
        Assert.Equal(
            WateringCommandStatus.Expired,
            await fixture.Context.WateringCommands.AsNoTracking()
                .Where(command => command.Id == expiredPending.CommandId)
                .Select(command => command.Status)
                .SingleAsync());
        Assert.DoesNotContain(activeCommands, command => command.WateringDeviceId == completed.DeviceId);
    }

    [Fact]
    public async Task TryCompleteAsync_ReconcilesLateCompletionExactlyOnce()
    {
        var acknowledgedAtUtc = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc);
        await using var fixture = await RepositoryFixture.CreateAsync(acknowledgedAtUtc);
        await fixture.Repository.GetActiveForDeviceAsync(
            fixture.DeviceId,
            acknowledgedAtUtc.AddMinutes(3));

        var command = fixture.CreateCompletedCommand(acknowledgedAtUtc);
        var completed = await fixture.Repository.TryCompleteAsync(command, fixture.CreateWateringEvent(acknowledgedAtUtc));
        var duplicate = await fixture.Repository.TryCompleteAsync(command, fixture.CreateWateringEvent(acknowledgedAtUtc));

        Assert.True(completed);
        Assert.False(duplicate);
        Assert.Equal(WateringCommandStatus.Completed, await fixture.GetCommandStatusAsync());
        Assert.Equal(1, await fixture.Context.WateringEvents.AsNoTracking().CountAsync());
    }

    [Fact]
    public async Task TryFailAsync_ReconcilesLateFailure()
    {
        var acknowledgedAtUtc = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc);
        await using var fixture = await RepositoryFixture.CreateAsync(acknowledgedAtUtc);
        await fixture.Repository.GetActiveForDeviceAsync(
            fixture.DeviceId,
            acknowledgedAtUtc.AddMinutes(3));
        var command = fixture.CreateCompletedCommand(acknowledgedAtUtc);
        command.Status = WateringCommandStatus.Failed;
        command.FailureReason = "Pump fault";

        var failed = await fixture.Repository.TryFailAsync(command);

        Assert.True(failed);
        var stored = await fixture.Context.WateringCommands.AsNoTracking().SingleAsync();
        Assert.Equal(WateringCommandStatus.Failed, stored.Status);
        Assert.Equal("Pump fault", stored.FailureReason);
    }

    private sealed class RepositoryFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private RepositoryFixture(SqliteConnection connection, PlantsContext context, int deviceId, int commandId, int plantId)
        {
            _connection = connection;
            Context = context;
            DeviceId = deviceId;
            CommandId = commandId;
            PlantId = plantId;
            Repository = new WateringCommandRepository(context);
        }

        public PlantsContext Context { get; }
        public WateringCommandRepository Repository { get; }
        public int DeviceId { get; }
        public int CommandId { get; }
        public int PlantId { get; }

        public static async Task<RepositoryFixture> CreateAsync(DateTime acknowledgedAtUtc)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<PlantsContext>()
                .UseSqlite(connection)
                .Options;
            var context = new PlantsContext(options);
            await context.Database.EnsureCreatedAsync();

            var plant = new Plant { Name = "Test plant", UserId = "user-1", WateringInterval = 7 };
            var device = new WateringDevice { UserId = "user-1", Plant = plant, WateringDurationSetting = 3 };
            var command = new WateringCommand
            {
                WateringDevice = device,
                Status = WateringCommandStatus.Acknowledged,
                DurationSeconds = 3,
                RequestedAtUtc = acknowledgedAtUtc.AddMinutes(-1),
                ExpiresAtUtc = acknowledgedAtUtc.AddMinutes(14),
                AcknowledgedAtUtc = acknowledgedAtUtc
            };
            context.WateringCommands.Add(command);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            return new RepositoryFixture(connection, context, device.Id, command.Id, plant.Id);
        }

        public WateringCommand CreateCompletedCommand(DateTime acknowledgedAtUtc)
        {
            return new WateringCommand
            {
                Id = CommandId,
                WateringDeviceId = DeviceId,
                Status = WateringCommandStatus.Completed,
                DurationSeconds = 3,
                RequestedAtUtc = acknowledgedAtUtc.AddMinutes(-1),
                ExpiresAtUtc = acknowledgedAtUtc.AddMinutes(14),
                AcknowledgedAtUtc = acknowledgedAtUtc,
                StartedAtUtc = acknowledgedAtUtc,
                FinishedAtUtc = acknowledgedAtUtc.AddSeconds(3)
            };
        }

        public WateringEvent CreateWateringEvent(DateTime acknowledgedAtUtc)
        {
            return new WateringEvent
            {
                PlantId = PlantId,
                Start = acknowledgedAtUtc,
                End = acknowledgedAtUtc.AddSeconds(3)
            };
        }

        public Task<WateringCommandStatus> GetCommandStatusAsync()
        {
            return Context.WateringCommands.AsNoTracking().Select(command => command.Status).SingleAsync();
        }

        public async Task<(int DeviceId, int CommandId)> AddCommandAsync(
            WateringCommandStatus status,
            DateTime expiresAtUtc)
        {
            var plant = new Plant { Name = $"Plant {status}", UserId = "user-1", WateringInterval = 7 };
            var device = new WateringDevice { UserId = "user-1", Plant = plant, WateringDurationSetting = 3 };
            var command = new WateringCommand
            {
                WateringDevice = device,
                Status = status,
                DurationSeconds = 3,
                RequestedAtUtc = expiresAtUtc.AddMinutes(-15),
                ExpiresAtUtc = expiresAtUtc
            };
            Context.WateringCommands.Add(command);
            await Context.SaveChangesAsync();
            Context.ChangeTracker.Clear();
            return (device.Id, command.Id);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
