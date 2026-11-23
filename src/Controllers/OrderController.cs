using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace basic_ecommerce_platform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> CreateOrder(CreateOrderDto dto)
        {
            var order = await _orderService.CreateOrderAsync(UserId, dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetAllOrders([FromQuery] int page = 1)
            => Ok(await _orderService.GetAllOrdersAsync(UserId, page));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
            => Ok(await _orderService.GetOrderByIdAsync(UserId, id));

        [HttpPost("{id:int}/pay")]
        public async Task<ActionResult<OrderDetailDto>> PayOrder(int id, PayOrderDto dto)
            => Ok(await _orderService.PayOrderAsync(UserId, id, dto));

        [HttpGet("~/api/admin/order")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminOrderSummaryDto>>> GetAllOrdersAsAdmin([FromQuery] AdminOrderQuery query)
            => Ok(await _orderService.GetAllOrdersAsAdminAsync(query));

        [HttpGet("~/api/admin/order/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminOrderDetailDto>> GetOrderByIdAsAdmin(int id)
            => Ok(await _orderService.GetOrderByIdAsAdminAsync(id));
    }
}
