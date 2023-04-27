using OrderModule.Data.DbContexts;
using OrderModule.Data.Repositories;

namespace OrderModule.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly OrderDbContext _orderDbContext;
        public IOrderRepository OrderRepository { get; set; }

        public RepositoryWrapper(OrderDbContext orderDbContext,
                                 IOrderRepository orderRepository)
        {
            _orderDbContext = orderDbContext;
            OrderRepository = orderRepository;
        }

        public async Task SaveAsync()
        {
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
