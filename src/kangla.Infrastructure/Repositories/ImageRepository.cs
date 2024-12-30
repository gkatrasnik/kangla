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

        public async Task<Image?> GetImageAsync(int imageId)
        {
            var image = await _context.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageId);
            return image;
        }

        public async Task<Image> AddImageAsync(Image image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ImageExistsAsync(int imageId)
        {
            return await _context.Images
                .AnyAsync(x => x.Id == imageId);
        }
    }
}