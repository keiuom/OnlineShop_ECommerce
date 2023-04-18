using BuyNow.Core;
using OrderModule.Core.Enums;

namespace OrderModule.Core.Domain
{
    public class Order : BaseEntity<int>
    {
        public string CustomerEmail { get; set; } = default!;

        public OrderStatusEnum Status { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = default!;
    }
}
