using OrderClosingWorkerService;
using OrderClosingWorkerService.Clients;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddSingleton<IHttpClientService, HttpClientService>();
    })
    .Build();

await host.RunAsync();
