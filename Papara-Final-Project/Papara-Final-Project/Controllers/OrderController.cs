using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Services;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [Authorize]
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
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDto)
        {
            try
            {
                // Token'dan kullanıcı ID'sini alıyoruz
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                await _orderService.AddOrder(orderDto, int.Parse(userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            await _orderService.UpdateOrder(id, orderDto);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrder(id);
            return Ok();
        }
    }
}
