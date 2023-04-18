using BuyNow.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderModule.Core.Domain
{
    public class OrderDetail : BaseEntity<int>
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = default!;
    }
}
