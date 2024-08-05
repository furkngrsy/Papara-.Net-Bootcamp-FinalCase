using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Services;
using System.Threading.Tasks;

namespace Papara_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCoupons();
            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return Ok(coupon);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCoupon([FromBody] CouponDTO couponDto)
        {
            await _couponService.AddCoupon(couponDto);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CouponDTO couponDto)
        {
            await _couponService.UpdateCoupon(id, couponDto);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            await _couponService.DeleteCoupon(id);
            return Ok();
        }
    }
}
