using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet("{imageId}")]
        public async Task<ActionResult<ImageResponseDto>> GetImage(int imageId)
        {
            var image = await _imageService.GetImageAsync(imageId);
            return Ok(image);
        }

        [Authorize]
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var deleted = await _imageService.DeleteImageAsync(imageId);
            if (!deleted)
            {
                return NotFound(new { message = $"Image with ID {imageId} not found." });
            }

            return NoContent();
        }
    }
}
