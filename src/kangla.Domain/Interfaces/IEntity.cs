using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Interfaces
{
    public class IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
