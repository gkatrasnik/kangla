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
            var expiredCommands = await _context.WateringCommands
                .Where(c => c.WateringDeviceId == deviceId
                    && c.Status == WateringCommandStatus.Pending
                    && c.ExpiresAtUtc <= nowUtc)
                .ToListAsync();

            if (expiredCommands.Count > 0)
            {
                foreach (var command in expiredCommands)
                {
                    command.Status = WateringCommandStatus.Expired;
                }

                await _context.SaveChangesAsync();
            }

            return await _context.WateringCommands
                .AsNoTracking()
                .Where(c => c.WateringDeviceId == deviceId
                    && (c.Status == WateringCommandStatus.Pending || c.Status == WateringCommandStatus.Acknowledged))
                .OrderBy(c => c.RequestedAtUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<WateringCommand?> GetByIdForDeviceAsync(int commandId, int deviceId)
        {
            return await _context.WateringCommands
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commandId && c.WateringDeviceId == deviceId);
        }

        public async Task<WateringCommand?> GetByIdForUserAsync(int commandId, int deviceId, string userId)
        {
            return await _context.WateringCommands
                .AsNoTracking()
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

        public async Task CompleteAsync(WateringCommand command, WateringEvent wateringEvent)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var existingCommand = await _context.WateringCommands.FirstOrDefaultAsync(c => c.Id == command.Id)
                ?? throw new KeyNotFoundException($"Watering command with ID {command.Id} was not found.");

            _context.Entry(existingCommand).CurrentValues.SetValues(command);
            _context.WateringEvents.Add(wateringEvent);
            await _context.SaveChangesAsync();

            existingCommand.WateringEventId = wateringEvent.Id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
