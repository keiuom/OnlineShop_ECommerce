namespace Inventory.Common.Models
{
    public record AddProductModel
    {
        public string Name { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
