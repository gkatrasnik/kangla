using Microsoft.AspNetCore.Http;

namespace kangla.Application.DTO
{
    public class PlantRecognizeRequestDto
    {
        public IFormFile? Image { get; set; }
    }
}
