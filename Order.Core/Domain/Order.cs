using BuyNow.Core;
using Order.Core.Enums;

namespace Order.Core.Domain
{
    public class Order : BaseEntity<int>
    {
        public string CustomerEmail { get; set; } = default!;

        public OrderStatusEnum Status { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = default!;
    }
}
