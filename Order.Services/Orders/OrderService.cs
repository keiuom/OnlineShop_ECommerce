using BuyNow.Core.Common;
using BuyNow.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Order.Common.DTOs;
using Order.Common.Enums;
using Order.Common.Models;
using OrderModule.Core.Domain;
using OrderModule.Data;
using System.Text;
using System.Text.Json;
using OrderEO = OrderModule.Core.Domain.Order;

namespace Order.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IRepositoryWrapper repository,
                           IHttpClientFactory httpClientFactory,
                           ILogger<OrderService> logger)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Response> PlaceOrderAsync(OrderModel orderModel)
        {
            if (!orderModel.Products.Any())
                return new Response { IsSuccess = false, Message = "No item added in the order list!", StatusCode = 400 };

            var order = PrepareOrderEntity(orderModel);

            await _repository.OrderRepository.AddAsync(order);
            await _repository.SaveAsync();

            return new Response { IsSuccess = true, Message = "Order placed successfully!" };
        }

        public async Task<OrderListDto> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var order = await _repository.OrderRepository.GetOrdersAsync(pageNumber, pageSize);

            var paging = new PagingHeader(order.CurrentPage, order.PageSize, order.TotalCount, order.TotalPages);

            return new OrderListDto
            {
                Orders = PrepareOrderDtos(order.ToList()),
                Paging = paging,
            };
        }

        public async Task CloseOrderAsync(int orderId)
        {
            var order = await _repository.OrderRepository
                    .GetOrderByIdAsync(orderId);

            if (order is null)
                throw new NotFoundException(nameof(OrderEO));

            var client = _httpClientFactory.CreateClient();

            var productCheckModel = PrepareProductAvailablityCheckModel(order.OrderDetails);
            var body = JsonSerializer.Serialize(productCheckModel);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var url = "https://localhost:7097/api/Products/ProductAvailablityCheck/";

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Response>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (responseData is not null && (responseData.IsSuccess && responseData.StatusCode == 200))
                {
                    await UpdateOrderStatus(order, responseData);
                    await UpdateProductsQuantity(productCheckModel);
                }
            }
            else
            {
                _logger.LogError("Not able to proceed this order, something went wrong!");
            }
        }

        private async Task UpdateOrderStatus(OrderEO order, Response response)
        {
            order.Status = OrderModule.Core.Enums.OrderStatusEnum.Closed;
            order.LastUpdatedAt = DateTime.UtcNow;

            _repository.OrderRepository.Edit(order);
            await _repository.SaveAsync();
        }

        private async Task UpdateProductsQuantity(List<ProductAvailablityModel> products)
        {
            var client = _httpClientFactory.CreateClient();
            var body = JsonSerializer.Serialize(products);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var url = "https://localhost:7097/api/Products/ProductQuantity/";

            var response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Products quantities updated successfully!");
            }
            else
            {
                _logger.LogError("Products quantities failed to update!");
            }
        }

        private List<ProductAvailablityModel> PrepareProductAvailablityCheckModel(List<OrderDetail> orderDetails)
        {
            var productList = new List<ProductAvailablityModel>();

            orderDetails.ForEach(od =>
            {
                var product = new ProductAvailablityModel
                {
                    Id = od.ProductId,
                    Quantity = od.Quantity,
                };

                productList.Add(product);
            });

            return productList;
        }

        private List<OrderDto> PrepareOrderDtos(List<OrderEO> orders)
        {
            var orderDtos = new List<OrderDto>();

            orders.ForEach(order =>
            {
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    CustomerEmail = order.CustomerEmail,
                    OrderStatus = (OrderStatus)order.Status,
                    PlacedDate = order.CreatedAt
                };

                orderDtos.Add(orderDto);
            });

            return orderDtos;
        }

        private OrderEO PrepareOrderEntity(OrderModel orderModel)
        {
            var dateTime = DateTime.UtcNow;

            var order = new OrderEO
            {
                CustomerEmail = orderModel.Email,
                Status = OrderModule.Core.Enums.OrderStatusEnum.Pending,
                OrderDetails = PrepareOrderDetailList(orderModel.Products),
                CreatedAt = dateTime,
                LastUpdatedAt = dateTime
            };

            return order;
        }

        private List<OrderDetail> PrepareOrderDetailList(List<ProductModel> products)
        {
            var orderDetails = new List<OrderDetail>();

            var dateTime = DateTime.UtcNow;

            products.ForEach(product =>
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = product.Quantity,
                    CreatedAt = dateTime,
                    LastUpdatedAt = dateTime,
                };

                orderDetails.Add(orderDetail);
            });

            return orderDetails;
        }
    }
}
