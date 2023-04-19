using Order.Data.Repositories;
using OrderModule.Data.Repositories;

namespace OrderModule.Data
{
    public interface IRepositoryWrapper
    {
        public IOrderRepository OrderRepository { get; set; }

        public IEmailMessageRepository EmailMessageRepository { get; set; }

        Task SaveAsync();
    }
}
