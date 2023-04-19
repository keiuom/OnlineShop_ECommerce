using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Services.Mails;
using OrderModule.Data;

namespace Order.Services.WorkerServices
{
    public class EmailWorker : BackgroundService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailWorker> _logger;

        public EmailWorker(IRepositoryWrapper repository,
                           IEmailSender emailSender,
                           ILogger<EmailWorker> logger)
        {
            _repository = repository;
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var unsentMessages = await _repository.EmailMessageRepository
                        .GetAsync(em => !em.IsSent && em.SentCount < 4);

                foreach (var message in unsentMessages)
                {
                    try
                    {
                        await _emailSender.SendAsync(message.Recipient, message.Subject, message.Body);

                        message.IsSent = true;
                        message.SentAt = DateTime.UtcNow;

                        _repository.EmailMessageRepository.Edit(message);
                        await _repository.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
