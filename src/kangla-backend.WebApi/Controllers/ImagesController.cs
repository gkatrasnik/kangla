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
        private readonly ILogger<WateringDevicesController> _logger;
        private readonly IImageService _imageService;

        public ImagesController(ILogger<WateringDevicesController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet("{imageId}")]
        public async Task<ActionResult<ImageResponseDto>> GetImage(int imageId)
        {            
            var image = await _imageService.GetImageAsync(imageId);
            return Ok(image);
        }
    }
}
