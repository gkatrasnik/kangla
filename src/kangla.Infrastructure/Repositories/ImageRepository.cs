using kangla.Domain.Entities;
using kangla.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace kangla.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly PlantsContext _context;

        public ImageRepository(PlantsContext context)
        {
            _context = context;
        }

        public async Task<MediaImage?> GetImageAsync(Guid imageId, string userId)
        {
            var image = await _context.Images.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == imageId && x.UserId == userId);
            return image;
        }

        public async Task<MediaImage> AddImageAsync(MediaImage image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<bool> DeleteImageAsync(Guid imageId, string userId)
        {
            var image = await _context.Images
                .FirstOrDefaultAsync(x => x.Id == imageId && x.UserId == userId);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<string?> GetImageETagAsync(Guid imageId, string userId)
        {             
            return await _context.Images
                .AsNoTracking()
                .Where(x => x.Id == imageId && x.UserId == userId)
                .Select(x => x.ETag)
                .FirstOrDefaultAsync();
        }
    }
}
