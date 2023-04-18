using OrderModule.Data.Repositories;

namespace OrderModule.Data
{
    public interface IRepositoryWrapper
    {
        public IOrderRepository OrderRepository { get; set; }

        Task SaveAsync();
    }
}
