using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace kangla.Infrastructure.Services
{
    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly PlantsContext _context;
        private readonly ILogger<DatabaseMigrationService> _logger;

        public DatabaseMigrationService(PlantsContext context, ILogger<DatabaseMigrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void MigrateDatabase()
        {
            try
            {
                _context.Database.Migrate();
                _logger.LogInformation("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the database.");
                throw;
            }
        }
    }
}
