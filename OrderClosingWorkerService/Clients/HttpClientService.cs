using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace OrderClosingWorkerService.Clients
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public HttpClientService(IHttpClientFactory httpClientFactory, ILogger<HttpClientService> logger)
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

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
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
