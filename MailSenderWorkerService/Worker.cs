using Order.Services.Mails;

namespace MailSenderWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger,
                      IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _emailMessageService = scope.ServiceProvider.GetRequiredService<IEmailMessageService>();
                    var _emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    var unsentMessages = await _emailMessageService.GetAllUnsentMessagesAsync();

                    foreach (var message in unsentMessages)
                    {
                        try
                        {
                            await _emailSender.SendAsync(new MailRequest
                            {
                                ToEmail = message.Recipient,
                                Subject = message.Subject,
                                Body = message.Body
                            });

                            message.IsSent = true;
                            message.SentCount += 1;
                            message.SentAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            message.SentCount += 1;
                            _logger.LogError(ex.Message, ex);
                        }
                    }

                    await _emailMessageService.UpdateMessagesAsync(unsentMessages);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}