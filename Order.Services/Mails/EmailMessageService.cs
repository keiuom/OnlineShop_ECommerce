using Order.Common.Models;
using Order.Core.Domain;
using Order.Data.RepositoryWrappers;

namespace Order.Services.Mails
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IQueueMailRepositoryWrapper _repository;

        public EmailMessageService(IQueueMailRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task AddMessageAsync(AddEmailMessageModel messageModel)
        {
            var dateTime = DateTime.UtcNow;

            var message = new EmailMessage
            {
                Recipient = messageModel.Recipient,
                Subject = messageModel.Subject,
                Body = messageModel.Body,
                CreatedAt = dateTime,
                LastUpdatedAt = dateTime,
            };

            await _repository.EmailMessageRepository.AddAsync(message);
            await _repository.SaveAsync();
        }

        public async Task<List<EmailMessage>> GetAllUnsentMessagesAsync()
        {
            return (await _repository.EmailMessageRepository
                    .GetAsync(em => !em.IsSent && em.SentCount < 4))
                    .ToList();
        }

        public async Task UpdateMessageAsync(EmailMessage emailMessage)
        {
           _repository.EmailMessageRepository.Edit(emailMessage);
            await _repository.SaveAsync();
        }

        public async Task UpdateMessagesAsync(List<EmailMessage> emailMessages)
        {
            _repository.EmailMessageRepository.EditRange(emailMessages);
            await _repository.SaveAsync();
        }
    }
}
