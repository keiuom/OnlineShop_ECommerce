using MailSenderWorkerService;
using Order.Core.Settings;
using OrderModule.Data;
using OrderModule.Services;

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
        services.AddPersistence(hostingContext.Configuration);
        services.LoadDependency();
    })
    .Build();

await host.RunAsync();
