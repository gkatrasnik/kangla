using kangla.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kangla.WebApi.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult> GetImage(int imageId)
        {
            var image = await _imageService.GetImageAsync(imageId);

            var eTag = _imageService.GenerateETag(image.Data);

            if (Request.Headers["If-None-Match"] == eTag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            Response.Headers.Append("Cache-Control", "private,max-age=300");
            Response.Headers.Append("ETag", eTag);

            return File(image.Data, image.ContentType);
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
