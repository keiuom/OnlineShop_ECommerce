using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Order.Common.Models;
using Order.Core.Settings;
using Order.Services.Mails;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QueueMailSenderWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static ConnectionFactory? _factory;
        private static IConnection? _connection;
        private readonly RabbitMQSettings _rabbitMqSettings;

        private const string QueueName = "EmailSendingQueue";

        public Worker(ILogger<Worker> logger,
                      IOptions<RabbitMQSettings> rabbitMqSettingsOptions,
                      IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _rabbitMqSettings = rabbitMqSettingsOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                ProcessQueue();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private void ProcessQueue()
        {
            _factory = new ConnectionFactory { HostName = _rabbitMqSettings.HostName, UserName = _rabbitMqSettings.UserName, Password = _rabbitMqSettings.Password };

            using (_connection = _factory.CreateConnection())
            {
                using var channel = _connection.CreateModel();
                channel.QueueDeclare(QueueName, true, false, false, null);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(QueueName, true, consumer);

                consumer.Received += (model, ea) =>
                {
                    // Process the incoming message
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received message: {message}");
                    var emailModel = JsonConvert.DeserializeObject<QueueEmailModel>(message);

                    if (emailModel is not null)
                    {
                        SendEmailMessage(emailModel).ConfigureAwait(false).GetAwaiter();
                        Console.WriteLine($"Send email message at {DateTime.Now}");

                        //channel?.BasicAck(ea.DeliveryTag, false);
                    }
                };
            }
        }

        private async Task SendEmailMessage(QueueEmailModel emailModel)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                try
                {
                    await _emailSender.SendAsync(new MailRequest
                    {
                        ToEmail = emailModel.Recipient,
                        Subject = emailModel.Subject,
                        Body = emailModel.Body
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }
    }
}