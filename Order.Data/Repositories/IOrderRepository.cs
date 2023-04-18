using BuyNow.Data;
using orderEntity = OrderModule.Core.Domain;

namespace OrderModule.Data.Repositories
{
    public interface IOrderRepository : IRepository<orderEntity.Order, int>
    {
    }
}
