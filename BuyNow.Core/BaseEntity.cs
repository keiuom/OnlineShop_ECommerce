using System.ComponentModel.DataAnnotations;

namespace BuyNow.Core
{
    public abstract class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}
