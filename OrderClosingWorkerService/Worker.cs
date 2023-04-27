using OrderClosingWorkerService.Clients;

namespace OrderClosingWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl = "https://localhost:7097/api/Orders";

        public Worker(ILogger<Worker> logger,
                      IHttpClientService httpClientService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pendingOrderIds = await GetPendingOrders();

                if(pendingOrderIds is not null && pendingOrderIds.Any())
                {
                    var baseUrl = $"{_baseUrl}/CloseOrder";

                    foreach (var pendingOrderId in pendingOrderIds)
                    {
                        var request = PrepareHttpRequestMessageForConfirmOrder(baseUrl, pendingOrderId);
                        var resposeData = await _httpClientService.SendRequestAsync<object>(request);

                        //if (response.IsSuccessStatusCode)
                        //{
                        //    _logger.LogInformation("Order completed successfully!");
                        //}
                    }
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<List<int>?> GetPendingOrders()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_baseUrl}/PendingOrders"),
                Method = HttpMethod.Get,
            };

            return await _httpClientService.SendRequestAsync<List<int>>(request);
        }

        private HttpRequestMessage PrepareHttpRequestMessageForConfirmOrder(string url, int orderId)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{url}/{orderId}"),
                Method = HttpMethod.Put,
            };

            return request;
        }
    }
}