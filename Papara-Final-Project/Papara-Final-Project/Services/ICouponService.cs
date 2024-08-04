using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public interface ICouponService
    {
        Coupon GetCouponById(int id);
        IEnumerable<Coupon> GetAllCoupons();
        void AddCoupon(Coupon coupon);
        void UpdateCoupon(Coupon coupon);
        void DeleteCoupon(int id);
    }
}
