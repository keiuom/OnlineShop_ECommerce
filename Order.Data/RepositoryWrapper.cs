using Order.Data.Repositories;
using OrderModule.Data.DbContexts;
using OrderModule.Data.Repositories;

namespace OrderModule.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly OrderDbContext _orderDbContext;
        public IOrderRepository OrderRepository { get; set; }
        public IEmailMessageRepository EmailMessageRepository { get; set; }

        public RepositoryWrapper(OrderDbContext orderDbContext,
                                 IOrderRepository orderRepository,
                                 IEmailMessageRepository emailMessageRepository)
        {
            _orderDbContext = orderDbContext;
            OrderRepository = orderRepository;
            EmailMessageRepository = emailMessageRepository;
        }

        public async Task SaveAsync()
        {
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
