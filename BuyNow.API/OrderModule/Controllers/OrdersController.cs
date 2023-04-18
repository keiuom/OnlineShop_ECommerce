using BuyNow.API.Helpers;
using BuyNow.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Order.Common.DTOs;
using Order.Common.Models;
using Order.Services.Orders;

namespace BuyNow.API.OrderModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<Response> PlaceOrderAsync(OrderModel orderModel)
        {
            return await _orderService.PlaceOrderAsync(orderModel);
        }

        [HttpGet]
        public async Task<OrderListDto> GetOrdersAsync([FromQuery] OrderParam orderParam)
        {
            var orderListDto = await _orderService.GetOrdersAsync(orderParam.PageNumber, orderParam.PageSize);

            Response.AddPagination(orderListDto.Paging.CurrentPage, orderListDto.Paging.ItemsPerPage, orderListDto.Paging.TotalItems, orderListDto.Paging.TotalPages);

            return orderListDto;
        }
    }
}
