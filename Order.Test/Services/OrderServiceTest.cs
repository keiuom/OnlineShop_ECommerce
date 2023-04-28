using Autofac.Extras.Moq;
using Moq;
using Order.Common.Models;
using Order.Services.HttpClients;
using Order.Services.Mails;
using Order.Services.Orders;
using OrderModule.Core.Domain;
using OrderModule.Data;
using OrderModule.Data.Repositories;
using Shouldly;
using OrderEO = OrderModule.Core.Domain.Order;

namespace Order.Test.Services
{
    [TestFixture]
    public class OrderServiceTest
    {
        private AutoMock _autoMock;
        private Mock<IRepositoryWrapper> _repositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IEmailMessageService> _emailMessageServiceMock;
        private Mock<IOrderClient> _orderClientMock;
        private IOrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _autoMock = AutoMock.GetLoose();
            _orderService = _autoMock.Create<OrderService>();
            _repositoryMock = _autoMock.Mock<IRepositoryWrapper>();
            _orderRepositoryMock = _autoMock.Mock<IOrderRepository>();
            _emailMessageServiceMock = _autoMock.Mock<IEmailMessageService>();
            _orderClientMock = _autoMock.Mock<IOrderClient>();
        }

        [TearDown]
        public void Teardown() { }

        [Test]
        public async Task PlaceOrderAsync_WithValidOrder_ReturnsSuccessResponse()
        {
            // Arrange
            var orderModel = GetOrderModel();

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(r => r.OrderRepository.AddAsync(It.IsAny<OrderEO>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var response = await _orderService.PlaceOrderAsync(orderModel);

            // Assert
            response.ShouldSatisfyAllConditions(
                response => _repositoryMock.Verify(),
                response => response.IsSuccess.ShouldBeTrue(),
                response => response.Message.ShouldBe("Order placed successfully!")
                );
        }

        [Test]
        public async Task PlaceOrderAsync_WithNoItems_ReturnsErrorResponse()
        {
            // Arrange
            var orderModel = new OrderModel { Products = new List<ProductModel>() };

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            // Act
            var response = await _orderService.PlaceOrderAsync(orderModel);

            // Assert
            response.ShouldSatisfyAllConditions(
                response => response.IsSuccess.ShouldBeFalse(),
                response => response.Message.ShouldBe("No item added in the order list!"),
                response => response.StatusCode.ShouldBe(400)
                );
        }

        [Test]
        public async Task PlaceOrderAsync_WithValidOrder_CallsAddAsyncWithCorrectOrder()
        {
            // Arrange
            var orderModel = GetOrderModel();

            var order = new OrderEO
            {
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 1, Quantity = 2 },
                    new OrderDetail { ProductId = 2, Quantity = 1 },
                },
            };

            _repositoryMock.Setup(r => r.OrderRepository.AddAsync(It.IsAny<OrderEO>()))
                .Callback<OrderEO>(o => o.ShouldSatisfyAllConditions(
                    () => o.OrderDetails.Count.ShouldBe(order.OrderDetails.Count),
                    () => o.OrderDetails.First().ProductId.ShouldBe(order.OrderDetails.First().Id),
                    () => o.OrderDetails.First().Quantity.ShouldBe(order.OrderDetails.First().Quantity),
                    () => o.OrderDetails.Last().ProductId.ShouldBe(order.OrderDetails.Last().Id),
                    () => o.OrderDetails.Last().Quantity.ShouldBe(order.OrderDetails.Last().Quantity)
                )).Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(r => r.OrderRepository.AddAsync(It.IsAny<OrderEO>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var response = await _orderService.PlaceOrderAsync(orderModel);

            // Assert
            _repositoryMock.Verify(r => r.OrderRepository.AddAsync(It.IsAny<OrderEO>()), Times.Once);
        }

        [Test]
        public async Task PlaceOrderAsync_WithValidOrder_CallsSaveAsync()
        {
            // Arrange
            var orderModel = GetOrderModel();

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(r => r.OrderRepository.AddAsync(It.IsAny<OrderEO>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var response = await _orderService.PlaceOrderAsync(orderModel);

            // Assert
            _repositoryMock
               .Verify(x => x.OrderRepository.AddAsync(It.IsAny<OrderEO>()));

            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        private OrderModel GetOrderModel()
        {
            return new OrderModel
            {
                Products = new List<ProductModel>
                {
                    new ProductModel { Id = 1, Quantity = 2 },
                    new ProductModel { Id = 2, Quantity = 1 },
                },
            };
        }

    }
}
