using Domain.Entities;
using Domain.Model;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class PlantsRepository : IPlantsRepository
{
    private readonly WateringContext _context;

    public PlantsRepository(WateringContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<Plant>> GetPlantsAsync(string userId, int pageNumber, int pageSize)
    {
        var totalRecords = await _context.Plants.AsNoTracking()
            .Where(w => w.UserId == userId)
            .CountAsync();

        var plants = await _context.Plants.AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<Plant>(plants, pageNumber, pageSize, totalRecords);
    }

    public async Task<Plant?> GetPlantByIdAsync(int plantId, string userId)
    {
        return await _context.Plants.AsNoTracking()
            .Where(d => d.Id == plantId && d.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task AddPlantAsync(Plant plant)
    {
        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlantAsync(Plant plant, string userId)
    {
        if (plant == null)
        {
            throw new ArgumentNullException(nameof(plant));
        }

        var existingPlant = await _context.Plants
            .FirstOrDefaultAsync(d => d.Id == plant.Id && d.UserId == userId);

        if (existingPlant == null)
        {
            throw new InvalidOperationException($"Plant with Id {plant.Id} does not exist for current user.");
        }

        _context.Entry(existingPlant).CurrentValues.SetValues(plant);

        await _context.SaveChangesAsync();
    }

    public async Task DeletePlantAsync(int plantId)
    {
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant != null)
        {
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> PlantExistsAsync(int plantId)
    {
        return await _context.Plants.AnyAsync(e => e.Id == plantId);
    }

    public async Task<bool> PlantExistsForUserAsync(int plantId, string userId)
    {
        return await _context.Plants.AnyAsync(e => e.Id == plantId && e.UserId == userId);
    }
}
