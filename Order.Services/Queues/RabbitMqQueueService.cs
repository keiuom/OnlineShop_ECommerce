using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Order.Common.Models;
using Order.Core.Settings;
using RabbitMQ.Client;
using System.Text;

namespace Order.Services.Queues
{
    public class RabbitMqQueueService : IQueueService
    {
        private static ConnectionFactory? _factory;
        private static IConnection? _connection;
        private static IModel? _model;
        private readonly RabbitMQSettings _rabbitMqSettings;

        private const string QueueName = "EmailSendingQueue";

        public RabbitMqQueueService(IOptions<RabbitMQSettings> rabbitMqSettingsOptions)
        {
            _rabbitMqSettings = rabbitMqSettingsOptions.Value;
        }

        public void SendMessage(QueueEmailModel emailModel)
        {
            CreateConnection();
            var emailModelString = JsonConvert.SerializeObject(emailModel);
            var emailModelByteArray = Encoding.UTF8.GetBytes(emailModelString);
            _model.BasicPublish("", QueueName, null, emailModelByteArray);
        }

        private void CreateConnection()
        {
            _factory = new ConnectionFactory { HostName = _rabbitMqSettings.HostName, UserName = _rabbitMqSettings.UserName, Password = _rabbitMqSettings.Password };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.QueueDeclare(QueueName, true, false, false, null);
        }

    }
}
