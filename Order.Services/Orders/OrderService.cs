using BuyNow.Core.Common;
using BuyNow.Core.Helpers;
using Order.Common.DTOs;
using Order.Common.Enums;
using Order.Common.Models;
using OrderModule.Core.Domain;
using OrderModule.Data;
using OrderEO = OrderModule.Core.Domain.Order;

namespace Order.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repository;

        public OrderService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<Response> PlaceOrderAsync(OrderModel orderModel)
        {
            if(!orderModel.Products.Any())
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
