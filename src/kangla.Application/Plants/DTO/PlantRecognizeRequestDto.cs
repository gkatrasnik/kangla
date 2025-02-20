using Microsoft.AspNetCore.Http;

namespace kangla.Application.Plants.DTO
{
    public class PlantRecognizeRequestDto
    {
        public IFormFile? Image { get; set; }
    }
}
