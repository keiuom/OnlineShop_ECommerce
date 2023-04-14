using Inventory.Data.DbContexts;
using Inventory.Data.Repositories;

namespace Inventory.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly InventoryDbContext _inventoryDbContext;
        public IProductRepository ProductRepository { get; set; }

        public RepositoryWrapper(InventoryDbContext inventoryDbContext,
                                 IProductRepository productRepository)
        {
            _inventoryDbContext = inventoryDbContext;
            ProductRepository = productRepository;
        }

        public async Task SaveAsync()
        {
            await _inventoryDbContext.SaveChangesAsync();
        }
    }
}
