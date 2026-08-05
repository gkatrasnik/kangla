using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Repositories
{
    public class WateringCommandRepository : IWateringCommandRepository
    {
        private readonly PlantsContext _context;

        public WateringCommandRepository(PlantsContext context)
        {
            _context = context;
        }

        public async Task<WateringCommand?> GetActiveForDeviceAsync(int deviceId, DateTime nowUtc)
        {
            var activeCommands = await GetActiveForDevicesAsync(new[] { deviceId }, nowUtc);
            return activeCommands.FirstOrDefault();
        }

        public async Task<IReadOnlyCollection<WateringCommand>> GetActiveForDevicesAsync(
            IReadOnlyCollection<int> deviceIds,
            DateTime nowUtc)
        {
            if (deviceIds.Count == 0)
            {
                return Array.Empty<WateringCommand>();
            }

            var distinctDeviceIds = deviceIds.Distinct().ToArray();
            var candidates = await _context.WateringCommands
                .AsNoTracking()
                .Where(c => distinctDeviceIds.Contains(c.WateringDeviceId)
                    && (c.Status == WateringCommandStatus.Pending || c.Status == WateringCommandStatus.Acknowledged))
                .ToListAsync();

            var expiredCommandIds = candidates
                .Where(c => c.Status == WateringCommandStatus.Pending && c.ExpiresAtUtc <= nowUtc)
                .Select(c => c.Id)
                .ToArray();
            if (expiredCommandIds.Length > 0)
            {
                await _context.WateringCommands
                    .Where(c => expiredCommandIds.Contains(c.Id) && c.Status == WateringCommandStatus.Pending)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Status, WateringCommandStatus.Expired));
            }

            var timedOutCommandIds = candidates
                .Where(c => c.Status == WateringCommandStatus.Acknowledged && HasAcknowledgementTimedOut(c, nowUtc))
                .Select(c => c.Id)
                .ToArray();
            if (timedOutCommandIds.Length > 0)
            {
                await _context.WateringCommands
                    .Where(c => timedOutCommandIds.Contains(c.Id) && c.Status == WateringCommandStatus.Acknowledged)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Status, WateringCommandStatus.TimedOut));
            }

            return await _context.WateringCommands
                .AsNoTracking()
                .Where(c => distinctDeviceIds.Contains(c.WateringDeviceId)
                    && (c.Status == WateringCommandStatus.Pending || c.Status == WateringCommandStatus.Acknowledged))
                .OrderBy(c => c.WateringDeviceId)
                .ThenBy(c => c.RequestedAtUtc)
                .ToListAsync();
        }

        public async Task<WateringCommand?> GetByIdForDeviceAsync(int commandId, int deviceId)
        {
            return await _context.WateringCommands
                .AsNoTracking()
                .Include(c => c.WateringDevice)
                .FirstOrDefaultAsync(c => c.Id == commandId && c.WateringDeviceId == deviceId);
        }

        public async Task<WateringCommand?> GetByIdForUserAsync(int commandId, int deviceId, string userId)
        {
            return await _context.WateringCommands
                .AsNoTracking()
                .Include(c => c.WateringDevice)
                .FirstOrDefaultAsync(c => c.Id == commandId && c.WateringDeviceId == deviceId && c.WateringDevice.UserId == userId);
        }

        public async Task<PagedResponse<WateringCommand>> GetForDeviceForUserAsync(int deviceId, string userId, int pageNumber, int pageSize)
        {
            var commands = _context.WateringCommands.AsNoTracking()
                .Where(c => c.WateringDeviceId == deviceId && c.WateringDevice.UserId == userId);

            var totalRecords = await commands.CountAsync();
            var page = await commands
                .OrderByDescending(c => c.RequestedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<WateringCommand>(page, pageNumber, pageSize, totalRecords);
        }

        public async Task<(WateringCommand Command, bool Created)> CreateOrGetActiveAsync(WateringCommand command, DateTime nowUtc)
        {
            _context.WateringCommands.Add(command);
            try
            {
                await _context.SaveChangesAsync();
                return (command, true);
            }
            catch (DbUpdateException)
            {
                _context.Entry(command).State = EntityState.Detached;
                var activeCommand = await GetActiveForDeviceAsync(command.WateringDeviceId, nowUtc);
                if (activeCommand is null)
                {
                    throw;
                }

                return (activeCommand, false);
            }
        }

        public async Task UpdateAsync(WateringCommand command)
        {
            var existingCommand = await _context.WateringCommands.FirstOrDefaultAsync(c => c.Id == command.Id)
                ?? throw new KeyNotFoundException($"Watering command with ID {command.Id} was not found.");

            _context.Entry(existingCommand).CurrentValues.SetValues(command);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TrySetStatusAsync(int commandId, WateringCommandStatus expectedStatus, WateringCommandStatus newStatus)
        {
            var affectedRows = await _context.WateringCommands
                .Where(c => c.Id == commandId && c.Status == expectedStatus)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Status, newStatus));

            return affectedRows == 1;
        }

        public async Task<bool> TryAcknowledgeAsync(int commandId, DateTime acknowledgedAtUtc)
        {
            var affectedRows = await _context.WateringCommands
                .Where(c => c.Id == commandId && c.Status == WateringCommandStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Status, WateringCommandStatus.Acknowledged)
                    .SetProperty(c => c.AcknowledgedAtUtc, acknowledgedAtUtc));

            return affectedRows == 1;
        }

        public async Task<bool> TryFailAsync(WateringCommand command)
        {
            var affectedRows = await _context.WateringCommands
                .Where(c => c.Id == command.Id
                    && (c.Status == WateringCommandStatus.Acknowledged || c.Status == WateringCommandStatus.TimedOut))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Status, WateringCommandStatus.Failed)
                    .SetProperty(c => c.StartedAtUtc, command.StartedAtUtc)
                    .SetProperty(c => c.FinishedAtUtc, command.FinishedAtUtc)
                    .SetProperty(c => c.FailureReason, command.FailureReason));

            return affectedRows == 1;
        }

        public async Task<bool> TryCompleteAsync(WateringCommand command, WateringEvent wateringEvent)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.WateringEvents.Add(wateringEvent);
            await _context.SaveChangesAsync();

            var affectedRows = await _context.WateringCommands
                .Where(c => c.Id == command.Id
                    && (c.Status == WateringCommandStatus.Acknowledged || c.Status == WateringCommandStatus.TimedOut))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Status, WateringCommandStatus.Completed)
                    .SetProperty(c => c.StartedAtUtc, command.StartedAtUtc)
                    .SetProperty(c => c.FinishedAtUtc, command.FinishedAtUtc)
                    .SetProperty(c => c.FailureReason, (string?)null)
                    .SetProperty(c => c.WateringEventId, wateringEvent.Id));

            if (affectedRows != 1)
            {
                await transaction.RollbackAsync();
                _context.Entry(wateringEvent).State = EntityState.Detached;
                return false;
            }

            await transaction.CommitAsync();
            command.WateringEventId = wateringEvent.Id;
            return true;
        }

        private static bool HasAcknowledgementTimedOut(WateringCommand command, DateTime nowUtc)
        {
            return command.AcknowledgedAtUtc.HasValue
                && command.AcknowledgedAtUtc.Value.AddSeconds(command.DurationSeconds).AddMinutes(2) <= nowUtc;
        }
    }
}
