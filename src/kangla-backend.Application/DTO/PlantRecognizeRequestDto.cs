using Microsoft.AspNetCore.Http;

namespace Application.DTO
{
    public class PlantRecognizeRequestDto
    {
        public IFormFile? Image { get; set; }
    }
}
