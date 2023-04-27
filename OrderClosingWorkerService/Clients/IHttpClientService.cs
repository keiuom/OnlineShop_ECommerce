namespace OrderClosingWorkerService.Clients
{
    public interface IHttpClientService
    {
        Task<T?> SendRequestAsync<T>(HttpRequestMessage request);
    }
}
