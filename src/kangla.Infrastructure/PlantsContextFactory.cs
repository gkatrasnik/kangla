using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kangla.Infrastructure
{
    /// <summary>
    /// Creates a design-time context for Entity Framework CLI commands without starting the Web API.
    /// This configuration is used only to scaffold and inspect migrations; the running application
    /// uses its configured connection string instead.
    /// </summary>
    public class PlantsContextFactory : IDesignTimeDbContextFactory<PlantsContext>
    {
        public PlantsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PlantsContext>();
            optionsBuilder.UseSqlite("Data Source=kangla-design-time.db");
            return new PlantsContext(optionsBuilder.Options);
        }
    }
}
