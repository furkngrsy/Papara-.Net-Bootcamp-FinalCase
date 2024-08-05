using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Papara_Final_Project.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coupon>> GetAllCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetCouponById(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task AddCoupon(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
        }

        public async Task UpdateCoupon(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
        }

        public async Task DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }
        }

        public async Task<Coupon> GetCouponByCode(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
        }
    }
}
