using Order.Core.Domain;
using OrderModule.Data;

namespace Order.Services.Mails
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IRepositoryWrapper _repository;

        public EmailMessageService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task AddMessageAsync(string recipient, string subject, string body)
        {
            var dateTime = DateTime.UtcNow;

            var message = new EmailMessage
            {
                Recipient = recipient,
                Subject = subject,
                Body = body,
                CreatedAt = dateTime,
                LastUpdatedAt = dateTime,
            };

            await _repository.EmailMessageRepository.AddAsync(message);
            await _repository.SaveAsync();
        }
    }
}
