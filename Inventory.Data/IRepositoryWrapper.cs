using Inventory.Data.Repositories;

namespace Inventory.Data
{
    public interface IRepositoryWrapper
    {
        IProductRepository ProductRepository { get; set; }

        Task SaveAsync();
    }
}
