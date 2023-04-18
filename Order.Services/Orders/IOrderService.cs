using BuyNow.Core.Common;
using BuyNow.Core.Helpers;
using Order.Common.DTOs;
using Order.Common.Models;
using OrderEO = OrderModule.Core.Domain.Order;

namespace Order.Services.Orders
{
    public interface IOrderService
    {
        Task<Response> PlaceOrderAsync(OrderModel orderModel);

        Task<OrderListDto> GetOrdersAsync(int pageNumber, int pageSize);
    }
}
