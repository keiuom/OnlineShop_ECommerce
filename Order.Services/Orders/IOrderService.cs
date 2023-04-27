using BuyNow.Core.Common;
using Order.Common.DTOs;
using Order.Common.Models;

namespace Order.Services.Orders
{
    public interface IOrderService
    {
        Task<Response> PlaceOrderAsync(OrderModel orderModel);

        Task<OrderListDto> GetOrdersAsync(int pageNumber, int pageSize);

        Task<List<int>> GetPendingOrderIdsAsync();

        Task CloseOrderAsync(int orderId);
    }
}
