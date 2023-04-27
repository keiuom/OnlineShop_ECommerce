using Order.Core.Domain;

namespace Order.Services.Mails
{
    public interface IEmailMessageService
    {
        Task<List<EmailMessage>> GetAllUnsentMessagesAsync();

        Task AddMessageAsync(string recipient, string subject, string body);

        Task UpdateMessageAsync(EmailMessage emailMessage);

        Task UpdateMessagesAsync(List<EmailMessage> emailMessages);
    }
}
