﻿using kangla.Domain.Entities;
using kangla.Domain.Model;

namespace kangla.Domain.Interfaces
{
    public interface IPlantsRepository
    {
        Task<PagedResponse<Plant>> GetPlantsAsync(string userId, int pageNumber, int pageSize);
        Task<Plant?> GetPlantByIdAsync(int plantId, string userId);
        Task AddPlantAsync(Plant plant);
        Task UpdatePlantAsync(Plant plant, string userId);
        Task DeletePlantAsync(int plantId);
        Task<bool> PlantExistsAsync(int plantId);
        Task<bool> PlantExistsForUserAsync(int plantId, string userId);
    }
}