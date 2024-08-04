using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;

namespace Papara_Final_Project.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public Coupon GetCouponById(int id)
        {
            return _couponRepository.GetCouponById(id);
        }

        public IEnumerable<Coupon> GetAllCoupons()
        {
            return _couponRepository.GetAllCoupons();
        }

        public void AddCoupon(Coupon coupon)
        {
            _couponRepository.AddCoupon(coupon);
        }

        public void UpdateCoupon(Coupon coupon)
        {
            _couponRepository.UpdateCoupon(coupon);
        }

        public void DeleteCoupon(int id)
        {
            _couponRepository.DeleteCoupon(id);
        }
    }
}
