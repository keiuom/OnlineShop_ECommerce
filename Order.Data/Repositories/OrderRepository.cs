using BuyNow.Data;
using Order.Data.DbContexts;
using orderEntity = Order.Core.Domain;


namespace Order.Data.Repositories
{
    public class OrderRepository : Repository<orderEntity.Order, int, OrderDbContext>, IOrderRepository
    {
        public OrderRepository(OrderDbContext orderDbContext)
            : base(orderDbContext) { }
    }
}
