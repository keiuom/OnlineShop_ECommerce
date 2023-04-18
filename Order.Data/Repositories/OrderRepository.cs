using BuyNow.Data;
using OrderModule.Core.Domain;
using OrderModule.Data.DbContexts;

namespace OrderModule.Data.Repositories
{
    public class OrderRepository : Repository<Order, int, OrderDbContext>, IOrderRepository
    {
        public OrderRepository(OrderDbContext orderDbContext)
            : base(orderDbContext) { }
    }
}
