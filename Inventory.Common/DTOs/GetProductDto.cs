namespace Inventory.Common.DTOs
{
    public record GetProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
