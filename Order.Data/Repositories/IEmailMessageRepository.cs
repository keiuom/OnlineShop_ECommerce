using BuyNow.Data;
using Order.Core.Domain;

namespace Order.Data.Repositories
{
    public interface IEmailMessageRepository : IRepository<EmailMessage, int>
    {
    }
}
