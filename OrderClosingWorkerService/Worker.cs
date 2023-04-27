using System.Text.Json;

namespace OrderClosingWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public Worker(ILogger<Worker> logger,
                      IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pendingOrderIds = await GetPendingOrders();

                if(pendingOrderIds is not null && pendingOrderIds.Any())
                {
                    var client = _clientFactory.CreateClient();
                    var baseUrl = $"https://localhost:7097/api/Orders/CloseOrder";

                    foreach (var pendingOrderId in pendingOrderIds)
                    {
                        var url = $"{baseUrl}/{pendingOrderId}";
                        var response = await client.PutAsync(url, null);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Order completed successfully!");
                        }
                    }
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<List<int>?> GetPendingOrders()
        {
            var client = _clientFactory.CreateClient();
            var url = "https://localhost:7097/api/Orders/PendingOrders/";
            var pendingOrderIds = new List<int>();

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                pendingOrderIds = JsonSerializer.Deserialize<List<int>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return pendingOrderIds;
        }
    }
}