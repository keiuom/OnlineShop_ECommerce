using BuyNow.Data;
using Inventory.Core.Domain;
using Inventory.Data.DbContexts;

namespace Inventory.Data.Repositories
{
    public class ProductRepository : Repository<Product, int, InventoryDbContext>, IProductRepository
    {
        public ProductRepository(InventoryDbContext inventoryDbContext)
            : base(inventoryDbContext) { }
    }
}
