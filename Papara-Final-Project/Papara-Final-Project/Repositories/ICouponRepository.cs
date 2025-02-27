﻿using Papara_Final_Project.Models;


namespace Papara_Final_Project.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllCoupons();
        Task<Coupon> GetCouponById(int id);
        Task AddCoupon(Coupon coupon);
        Task UpdateCoupon(Coupon coupon);
        Task DeleteCoupon(int id);
        Task<Coupon> GetCouponByCode(string code);
    }
}
