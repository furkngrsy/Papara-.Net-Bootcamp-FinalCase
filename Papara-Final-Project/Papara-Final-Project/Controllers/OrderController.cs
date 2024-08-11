using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Services;
using System.Security.Claims;

namespace Papara_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;


        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequestDTO orderDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                await _orderService.AddOrder(orderDto.Order, orderDto.Payment, int.Parse(userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrder(id);
            return Ok();
        }

        [Authorize]
        [HttpGet("order-details/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (order.UserId != userId)
            {
                return Unauthorized("You can't see this order.");

            }

            return Ok(order);
        }

        [Authorize]
        [HttpGet("order-product-details/{orderId}")]
        public async Task<IActionResult> GetOrderProductDetails(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var orderDetails = await _orderService.GetOrderProductDetails(orderId);

            if (orderDetails == null)
            {
                return NotFound("Order not found.");
            }

            var order = await _orderService.GetOrderById(orderId);

            if (order.UserId != userId)
            {
                return Unauthorized("You can't see this order.");
            }

            return Ok(orderDetails);
        }

        [Authorize]
        [HttpGet("active-orders")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var activeOrder = await _orderService.GetActiveOrders(userId);
            if (activeOrder == null)
            {
                return NotFound();
            }
            return Ok(activeOrder);
        }

        [Authorize]
        [HttpGet("inactive-orders")]
        public async Task<IActionResult> GetInactiveOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var inactiveOrder = await _orderService.GetInactiveOrders(userId);
            if (inactiveOrder == null)
            {
                return NotFound();
            }
            return Ok(inactiveOrder);
        }

    }
    public class OrderRequestDTO
    {
        public OrderDTO Order { get; set; }
        public PaymentDTO Payment { get; set; }
    }
}
