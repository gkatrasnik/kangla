using kangla.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Entities
{
    public class MediaImage : IEntity
    {
        [Required]
        public Guid Id { get; set; }
        public byte[] Data { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public string ETag { get; set; } = default!;
    }
}
