using BuyNow.Core.Helpers;
using BuyNow.Data;
using OrderEO = OrderModule.Core.Domain.Order;

namespace OrderModule.Data.Repositories
{
    public interface IOrderRepository : IRepository<OrderEO, int>
    {
        Task<PagedList<OrderEO>> GetOrdersAsync(int pageNumber, int pageSize);

        Task<OrderEO?> GetOrderByIdAsync(int id);
    }
}
