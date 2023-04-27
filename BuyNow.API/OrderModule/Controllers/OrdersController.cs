using BuyNow.API.Helpers;
using BuyNow.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Order.Common.DTOs;
using Order.Common.Models;
using Order.Services.Mails;
using Order.Services.Orders;

namespace BuyNow.API.OrderModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;

        public OrdersController(IOrderService orderService,
            IEmailSender emailSender)
        {
            _orderService = orderService;
            _emailSender = emailSender;
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

        [HttpPut("CloseOrder/{orderId}")]
        public async Task<IActionResult> CloseOrderAsync(int orderId)
        {
            await _orderService.CloseOrderAsync(orderId);

            return Ok();
        }
    }
}
