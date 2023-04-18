using Order.Common.Enums;

namespace Order.Common.DTOs
{
    public record OrderDto
    {
        public int Id { get; set; }

        public string CustomerEmail { get; set; } = default!;

        public OrderStatus OrderStatus { get; set; }

        public DateTime PlacedDate { get; set; }
    }
}
