using BuyNow.Data;
using OrderEO = OrderModule.Core.Domain.Order;
using OrderModule.Data.DbContexts;
using BuyNow.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace OrderModule.Data.Repositories
{
    public class OrderRepository : Repository<OrderEO, int, OrderDbContext>, IOrderRepository
    {
        public OrderRepository(OrderDbContext orderDbContext)
            : base(orderDbContext) { }

        public async Task<PagedList<OrderEO>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            IQueryable<OrderEO> orders = _dbContext.Orders;

            return await PagedList<OrderEO>.CreateAsync(orders, pageNumber, pageSize);
        }

        public async Task<OrderEO?> GetOrderByIdAsync(int id)
        {
            return await _dbContext.Orders
                    .Where(o => o.Id == id)
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync();
        }
    }
}
