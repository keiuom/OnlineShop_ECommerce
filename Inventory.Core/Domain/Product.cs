using BuyNow.Core;

namespace Inventory.Core.Domain
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
