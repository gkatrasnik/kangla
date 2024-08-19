using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure
{

    public class WateringContext : IdentityDbContext<IdentityUser>
    {
        public WateringContext(DbContextOptions<WateringContext> options)
        : base(options)
        {
        }

        public DbSet<WateringDevice> WateringDevices { get; set; }
        public DbSet<WateringEvent> WateringEvents { get; set; }
        public DbSet<HumidityMeasurement> HumidityMeasurements { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Image> Images { get; set; }

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

            modelBuilder.Entity<Plant>()
                .HasOne(w => w.WateringDevice)
                .WithOne(w => w.Plant)
                .HasForeignKey<WateringDevice>(w => w.PlantId);

            modelBuilder.Entity<Plant>()
                .HasOne(w => w.Image);

            modelBuilder.Entity<WateringDevice>()
                .HasMany(w => w.HumidityMeasurements)
                .WithOne(h => h.WateringDevice)
                .HasForeignKey(h => h.WateringDeviceId);

            modelBuilder.Entity<WateringDevice>()
                .HasIndex(d => d.DeviceToken)
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
