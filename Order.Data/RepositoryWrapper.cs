using OrderModule.Data.DbContexts;

namespace OrderModule.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly OrderDbContext _orderDbContext;

        public RepositoryWrapper(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task SaveAsync()
        {
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
