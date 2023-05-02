using Order.Data.DbContexts;
using Order.Data.Repositories;

namespace Order.Data.RepositoryWrappers
{
    public class QueueMailRepositoryWrapper : IQueueMailRepositoryWrapper
    {
        private readonly EmailDbContext _dbContext;

        public IEmailMessageRepository EmailMessageRepository { get; set; }

        public QueueMailRepositoryWrapper(EmailDbContext dbContext, IEmailMessageRepository emailMessageRepository)
        {
            _dbContext = dbContext;
            EmailMessageRepository = emailMessageRepository;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
