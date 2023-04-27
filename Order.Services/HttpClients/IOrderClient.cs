namespace Order.Services.HttpClients
{
    public interface IOrderClient
    {
        Task<T?> SendRequestAsync<T>(HttpRequestMessage request);
    }
}
