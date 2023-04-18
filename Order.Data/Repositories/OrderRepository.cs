using BuyNow.Data;
using orderEntity = OrderModule.Core.Domain;
using OrderModule.Data.DbContexts;

namespace OrderModule.Data.Repositories
{
    public class OrderRepository : Repository<orderEntity.Order, int, OrderDbContext>, IOrderRepository
    {
        public OrderRepository(OrderDbContext orderDbContext)
            : base(orderDbContext) { }
    }
}
