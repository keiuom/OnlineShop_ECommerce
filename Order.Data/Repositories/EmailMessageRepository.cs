using BuyNow.Data;
using Order.Core.Domain;
using Order.Data.DbContexts;

namespace Order.Data.Repositories
{
    public class EmailMessageRepository : Repository<EmailMessage, int, EmailDbContext>, IEmailMessageRepository
    {
        public EmailMessageRepository(EmailDbContext dbContext)
            : base(dbContext) { }
    }
}
