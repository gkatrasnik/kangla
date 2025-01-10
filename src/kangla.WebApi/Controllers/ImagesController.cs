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
        public async Task<ActionResult> GetImage(Guid imageId)
        {
            var eTag = await _imageService.GetImageETagAsync(imageId); 
            if (Request.Headers["If-None-Match"] == eTag) //current angular custom image src directive does not send If-None-Match header
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }
            
            var image = await _imageService.GetImageAsync(imageId);

            //315360001 - 1 year  - Images does not change, images can only be deleted or created.
            Response.Headers.Append("Cache-Control", "private, max-age=2592000"); // 1 month
            Response.Headers.Append("ETag", image.ETag);

            return File(image.Data, image.ContentType);
        }

        [Authorize]
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
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
