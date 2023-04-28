using BuyNow.Core.Common;
using BuyNow.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Order.Common.DTOs;
using Order.Common.Enums;
using Order.Common.Models;
using Order.Services.HttpClients;
using Order.Services.Mails;
using Order.Services.Mails.MailTemplates;
using OrderModule.Core.Domain;
using OrderModule.Core.Enums;
using OrderModule.Data;
using System.Text;
using OrderEO = OrderModule.Core.Domain.Order;

namespace Order.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ILogger<OrderService> _logger;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IOrderClient _orderClient;
        private const string _baseUrl = "https://localhost:7097/api/Products";

        public OrderService(IRepositoryWrapper repository,
                           ILogger<OrderService> logger,
                           IEmailMessageService emailMessageService,
                           IOrderClient orderClient)
        {
            _repository = repository;
            _logger = logger;
            _emailMessageService = emailMessageService;
            _orderClient = orderClient;
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

        public async Task<List<int>> GetPendingOrderIdsAsync()
        {
            return await _repository.OrderRepository.GetPendingOrderIdsAsync();
        }

        public async Task CloseOrderAsync(int orderId)
        {
            var order = await _repository.OrderRepository
                    .GetOrderByIdAsync(orderId);

            if (order is null)
                throw new NotFoundException(nameof(OrderEO));

            var productCheckModel = PrepareProductAvailablityCheckModel(order.OrderDetails);
            var request = PrepareRequestPayload(productCheckModel, "ProductAvailablityCheck", HttpMethod.Post);
            var responseData = await _orderClient.SendRequestAsync<Response>(request);

            if (responseData is not null && (responseData.IsSuccess && responseData.StatusCode == 200))
            {
                await UpdateOrderStatus(order, OrderStatusEnum.Closed);
                await AddOrderSuccessMessageToQueue(order.Id, order.CustomerEmail);
                await UpdateProductsQuantity(productCheckModel);
            }
            else
            {
                await UpdateOrderStatus(order, OrderStatusEnum.Closed);
                await AddOrderFailedMessageToQueue("Due to product unavailability, we are not able to process your order!", order.CustomerEmail);

                _logger.LogError("Not able to proceed this order, something went wrong!");
            }
        }

        private HttpRequestMessage PrepareRequestPayload(object data, string apiPath, HttpMethod httpMethod)
        {
            var serializedContent = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_baseUrl}/{apiPath}"),
                Method = httpMethod,
                Content = new StringContent(serializedContent, Encoding.UTF8, "application/json")
            };

            return request;
        }

        private async Task UpdateOrderStatus(OrderEO order, OrderStatusEnum orderStatus)
        {
            order.Status = orderStatus;
            order.LastUpdatedAt = DateTime.UtcNow;

            _repository.OrderRepository.Edit(order);
            await _repository.SaveAsync();
        }

        private async Task AddOrderSuccessMessageToQueue(int orderId, string customerEmail)
        {
            var emailTemplate = new OrderSuccessMailTemplate(orderId, "support@gmail.com");
            var subject = "Order status";
            var emailBody = emailTemplate.TransformText();
            await _emailMessageService.AddMessageAsync(customerEmail, subject, emailBody);
        }

        private async Task AddOrderFailedMessageToQueue(string failedReason, string customerEmail)
        {
            var emailTemplate = new OrderFailedMailTemplate(failedReason, "support@gmail.com");
            var subject = "Order status";
            var emailBody = emailTemplate.TransformText();
            await _emailMessageService.AddMessageAsync(customerEmail, subject, emailBody);
        }

        private async Task UpdateProductsQuantity(List<ProductAvailablityModel> products)
        {
            var request = PrepareRequestPayload(products, "ProductQuantity", HttpMethod.Put);
            var responseData = await _orderClient.SendRequestAsync<object>(request);

            //var client = _httpClientFactory.CreateClient();
            //var body = System.Text.Json.JsonSerializer.Serialize(products);
            //var content = new StringContent(body, Encoding.UTF8, "application/json");
            //var url = "https://localhost:7097/api/Products/ProductQuantity/";

            //var response = await client.PutAsync(url, content);

            //if (response.IsSuccessStatusCode)
            //{
            //    _logger.LogInformation("Products quantities updated successfully!");
            //}
            //else
            //{
            //    _logger.LogError("Products quantities failed to update!");
            //}
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
