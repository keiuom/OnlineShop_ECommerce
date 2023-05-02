using Order.Common.Models;
using Order.Core.Domain;

namespace Order.Services.Mails
{
    public interface IEmailMessageService
    {
        Task<List<EmailMessage>> GetAllUnsentMessagesAsync();

        Task AddMessageAsync(AddEmailMessageModel messageModel);

        Task UpdateMessageAsync(EmailMessage emailMessage);

        Task UpdateMessagesAsync(List<EmailMessage> emailMessages);
    }
}
