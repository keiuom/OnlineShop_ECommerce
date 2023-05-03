using Newtonsoft.Json;
using Order.Services.HttpClients;
using System.Net;
using System.Text;
using BuyNow.Core.Common;
using Polly.Retry;
using Polly;
using System.Net.Http;
using System.Net.Cache;
using NUnit.Framework;

namespace Order.Test.HttpClients
{
    [TestFixture]
    public class OrderClientTest
    {
        private AutoMock _autoMock;
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<IAsyncPolicy> _pollyRetryMock;
        private IOrderClient _orderClient;

        [SetUp] 
        public void Setup() 
        {
            _autoMock = AutoMock.GetLoose();
            _httpClientFactoryMock = _autoMock.Mock<IHttpClientFactory>();
            _httpClientMock = _autoMock.Mock<HttpClient>();
            _pollyRetryMock = _autoMock.Mock<IAsyncPolicy>();
            _orderClient = _autoMock.Create<OrderClient>();
        }

        [Test]
        [Ignore("")]
        public async Task SendRequestAsync_SuccessResponse_ReturnsDeserializedObject()
        {
            // Arrange
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.com");

            var expectedResponse = new Response { Message = "Value", IsSuccess = true };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
            };

            _pollyRetryMock.Setup(x => x.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()))
                .ReturnsAsync(httpResponseMessage);

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(_httpClientMock.Object);

            _httpClientMock.Setup(x => x.SendAsync(httpRequestMessage, CancellationToken.None))
                .ReturnsAsync(httpResponseMessage);

            // Act
            var actualResponse = await _orderClient.SendRequestAsync<Response>(httpRequestMessage);

            // Assert
            actualResponse?.IsSuccess.ShouldBeTrue();
            //actualResponse.ShouldBeEquivalentTo(expectedResponse);
        }
    }
}
