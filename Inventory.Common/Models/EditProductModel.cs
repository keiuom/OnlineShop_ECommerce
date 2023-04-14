namespace Inventory.Common.Models
{
    public record EditProductModel
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
    }
}
