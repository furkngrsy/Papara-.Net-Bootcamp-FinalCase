using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.Models;
using Papara_Final_Project.Services;

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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var coupons = _couponService.GetAllCoupons();
            return Ok(coupons);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var coupon = _couponService.GetCouponById(id);
            if (coupon == null)
                return NotFound();

            return Ok(coupon);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromBody] Coupon coupon)
        {
            _couponService.AddCoupon(coupon);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult Update([FromBody] Coupon coupon)
        {
            _couponService.UpdateCoupon(coupon);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _couponService.DeleteCoupon(id);
            return Ok();
        }
    }
}
