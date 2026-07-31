using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using kangla.Domain.Entities;
using kangla.Domain.Interfaces;

namespace kangla.Infrastructure
{

    public class PlantsContext : IdentityDbContext<IdentityUser>
    {
        public PlantsContext(DbContextOptions<PlantsContext> options)
        : base(options)
        {
        }

        public DbSet<WateringDevice> WateringDevices { get; set; }
        public DbSet<WateringEvent> WateringEvents { get; set; }
        public DbSet<WateringCommand> WateringCommands { get; set; }
        public DbSet<HumidityMeasurement> HumidityMeasurements { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<MediaImage> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureRelationships(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plant>()
                .HasMany(w => w.WateringEvents)
                .WithOne(w => w.Plant)
                .HasForeignKey(w => w.PlantId);

            modelBuilder.Entity<WateringDevice>()
                .HasOne(w => w.Plant)
                .WithOne(p => p.WateringDevice)
                .HasForeignKey<WateringDevice>(w => w.PlantId)
                .IsRequired(false);

            modelBuilder.Entity<WateringDevice>()
                .HasMany(w => w.HumidityMeasurements)
                .WithOne(h => h.WateringDevice)
                .HasForeignKey(h => h.WateringDeviceId);

            modelBuilder.Entity<WateringDevice>()
                .HasMany(w => w.WateringCommands)
                .WithOne(c => c.WateringDevice)
                .HasForeignKey(c => c.WateringDeviceId);

            modelBuilder.Entity<WateringCommand>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<WateringCommand>()
                .HasOne(c => c.WateringEvent)
                .WithOne()
                .HasForeignKey<WateringCommand>(c => c.WateringEventId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WateringCommand>()
                .HasIndex(c => new { c.WateringDeviceId, c.Status });

            modelBuilder.Entity<WateringCommand>()
                .HasIndex(c => c.Status);

            modelBuilder.Entity<WateringCommand>()
                .HasIndex(c => c.WateringDeviceId)
                .IsUnique()
                .HasFilter("\"Status\" IN ('Pending', 'Acknowledged')");

            modelBuilder.Entity<WateringDevice>()
                .HasIndex(d => d.DeviceAccessKeyHash)
                .IsUnique();
        }

        public Task<int> SaveChangesAsync()
        {
            UpdateTimestamps();
            return base.SaveChangesAsync();
        }

        private void UpdateTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }
        }
    }
}
