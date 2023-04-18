using Order.Common.Models;

namespace Order.Common.DTOs
{
    public class OrderListDto
    {
        public List<OrderDto> Orders { get; set; } = default!;

        public PagingHeader Paging { get; set; } = default!;
    }
}
