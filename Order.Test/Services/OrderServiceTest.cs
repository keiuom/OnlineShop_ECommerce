﻿using Autofac.Extras.Moq;
using BuyNow.Core.Common;
using BuyNow.Core.Helpers;
using Moq;
using Order.Common.DTOs;
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

        private const int _pageNumber = 1;
        private const int _pageSize = 10;
        private const int _totalCount = 10;
        private const string _customerEmail = "user1@gmail.com";

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

        [Test]
        public async Task GetOrdersAsync_ReturnsNonNullResult()
        {
            // Arrange
            var orders = new PagedList<OrderEO>(new List<OrderEO>(), _totalCount, _pageNumber, _pageSize);

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(r => r.OrderRepository.GetOrdersAsync(_pageNumber, _pageSize))
                .ReturnsAsync(orders)
                .Verifiable();

            // Act
            var result = await _orderService.GetOrdersAsync(_pageNumber, _pageSize);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => result.ShouldNotBeNull(),
                result => _repositoryMock.Verify()
                );
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsOrderListDto()
        {
            // Arrange
            var orders = new PagedList<OrderEO>(new List<OrderEO>(), _totalCount, _pageNumber, _pageSize);

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(r => r.OrderRepository.GetOrdersAsync(_pageNumber, _pageSize))
                .ReturnsAsync(orders)
                .Verifiable();

            // Act
            var result = await _orderService.GetOrdersAsync(_pageNumber, _pageSize);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => result.ShouldBeOfType<OrderListDto>(),
                result => _repositoryMock.Verify()
                );
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsCorrectNumberOfOrders()
        {
            // Arrange
            var orders = GetOrderEntityList();

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(x => x.OrderRepository.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedList<OrderEO>(orders, _totalCount, _pageNumber, _pageSize))
                .Verifiable();

            // Act
            var result = await _orderService.GetOrdersAsync(_pageNumber, _pageSize);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => result.Orders.Count.ShouldBeEquivalentTo(orders.Count),
                result => _repositoryMock.Verify()
                );
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsCorrectPagingInformation()
        {
            // Arrange
            var currentPage = 1;
            var pageSize = 10;
            var totalItems = 10;
            var orders = GetOrderEntityList();

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(x => x.OrderRepository.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedList<OrderEO>(orders, _totalCount, _pageNumber, _pageSize))
                .Verifiable();

            // Act
            var result = await _orderService.GetOrdersAsync(_pageNumber, _pageSize);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => result.Paging.CurrentPage.ShouldBeEquivalentTo(currentPage),
                result => result.Paging.ItemsPerPage.ShouldBeEquivalentTo(pageSize),
                result => result.Paging.TotalItems.ShouldBeEquivalentTo(totalItems),
                result => _repositoryMock.Verify()
                );
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsCorrectOrderInformation()
        {
            // Arrange
            var orders = GetOrderEntityList();

            _repositoryMock.Setup(r => r.OrderRepository)
                .Returns(_orderRepositoryMock.Object);

            _repositoryMock.Setup(x => x.OrderRepository.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedList<OrderEO>(orders, _totalCount, _pageNumber, _pageSize));

            // Act
            var result = await _orderService.GetOrdersAsync(_pageNumber, _pageSize);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => result.Orders[0].Id.ShouldBeEquivalentTo(orders[0].Id),
                result => result.Orders[0].CustomerEmail.ShouldBeEquivalentTo(orders[0].CustomerEmail),
                result => result.Orders[1].Id.ShouldBeEquivalentTo(orders[1].Id),
                result => _repositoryMock.Verify()
                );
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

        private List<OrderEO> GetOrderEntityList()
        {
           return new List<OrderEO>
            {
                new OrderEO { Id = 1, CustomerEmail = _customerEmail },
                new OrderEO { Id = 2, CustomerEmail = _customerEmail },
                new OrderEO { Id = 3, CustomerEmail = _customerEmail },
            };
        }

    }
}
