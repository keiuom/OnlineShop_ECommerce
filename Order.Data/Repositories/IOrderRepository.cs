using BuyNow.Data;
using orderEntity = Order.Core.Domain;

namespace Order.Data.Repositories
{
    public interface IOrderRepository : IRepository<orderEntity.Order, int>
    {
    }
}
