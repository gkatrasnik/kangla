using kangla.Domain.Interfaces;

namespace kangla.Domain.Entities
{
    public class MediaImage : IEntity
    {
        public byte[] Data { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public string ETag { get; set; } = default!;
    }
}
