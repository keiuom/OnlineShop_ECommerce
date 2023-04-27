using Order.Data.Repositories;
using OrderModule.Data.Repositories;

namespace Order.Data.RepositoryWrappers
{
    public interface lQueueMailRepositoryWrapper
    {
        public IEmailMessageRepository EmailMessageRepository { get; set; }

        Task SaveAsync();
    }
}
