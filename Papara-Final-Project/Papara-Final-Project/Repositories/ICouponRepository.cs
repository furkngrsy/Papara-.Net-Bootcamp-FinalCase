using Papara_Final_Project.Models;

namespace Papara_Final_Project.Repositories
{
    public interface ICouponRepository
    {
        Coupon GetCouponById(int id);
        IEnumerable<Coupon> GetAllCoupons();
        void AddCoupon(Coupon coupon);
        void UpdateCoupon(Coupon coupon);
        void DeleteCoupon(int id);
    }
}
