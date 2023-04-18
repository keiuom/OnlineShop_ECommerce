namespace Order.Common.Models
{
    public record OrderModel
    {
        public string Email { get; set; } = default!;

        public List<ProductModel> Products { get; set; } = default!;
    }
}
