namespace Application.DTO
{
    public class ImageResponseDto
    {
        public required int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageBase64 { get; set; }
    }
}
