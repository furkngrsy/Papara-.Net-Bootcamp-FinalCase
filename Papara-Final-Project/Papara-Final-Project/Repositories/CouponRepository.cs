using Papara_Final_Project.Models;

namespace Papara_Final_Project.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Coupon GetCouponById(int id)
        {
            return _context.Coupons.Find(id);
        }

        public IEnumerable<Coupon> GetAllCoupons()
        {
            return _context.Coupons.ToList();
        }

        public void AddCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
        }

        public void UpdateCoupon(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            _context.SaveChanges();
        }

        public void DeleteCoupon(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
            }
        }
    }
}
