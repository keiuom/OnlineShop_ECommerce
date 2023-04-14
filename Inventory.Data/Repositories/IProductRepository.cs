using BuyNow.Data;
using Inventory.Core.Domain;

namespace Inventory.Data.Repositories
{
    public interface IProductRepository : IRepository<Product, int>
    {
    }
}
