using BuyNow.Data;
using OrderModule.Core.Domain;

namespace OrderModule.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order, int>
    {
    }
}
