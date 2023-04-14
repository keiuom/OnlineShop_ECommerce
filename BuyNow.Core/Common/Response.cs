namespace BuyNow.Core.Common
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = default!;

        public int StatusCode { get; set; }
    }
}
