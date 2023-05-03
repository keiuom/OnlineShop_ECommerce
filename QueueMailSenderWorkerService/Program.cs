using Order.Core.Settings;
using OrderModule.Services;
using OrderModule.Data;

namespace QueueMailSenderWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHttpClient();

                    services.Configure<SmtpSettings>(hostingContext.Configuration.GetSection("SmtpSettings"));
                    services.Configure<RabbitMQSettings>(hostingContext.Configuration.GetSection("RabbitMQSettings"));
                    services.AddPersistence(hostingContext.Configuration);
                    services.LoadDependency();
                })
                .Build();

            host.Run();
        }
    }
}