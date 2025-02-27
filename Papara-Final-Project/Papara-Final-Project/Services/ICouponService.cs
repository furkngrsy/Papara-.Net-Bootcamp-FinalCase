﻿using Papara_Final_Project.DTOs;


namespace Papara_Final_Project.Services
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDTO>> GetAllCoupons();
        Task<CouponDTO> GetCouponById(int id);
        Task AddCoupon(CouponDTO couponDto);
        Task UpdateCoupon(int id, CouponDTO couponDto);
        Task DeleteCoupon(int id);
    }
}
