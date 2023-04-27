using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Order.Services.HttpClients
{
    public class OrderClient : IOrderClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrderClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public OrderClient(IHttpClientFactory httpClientFactory, ILogger<OrderClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Request failed with {result.Result.StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                    });
        }

        public async Task<T?> SendRequestAsync<T>(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var responseMessage = await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.SendAsync(request);
                return response;
            });

            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());
            }

            return default;
        }
    }
}
