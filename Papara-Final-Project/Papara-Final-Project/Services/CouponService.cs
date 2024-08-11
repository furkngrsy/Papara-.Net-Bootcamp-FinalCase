using FluentValidation;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;
using Papara_Final_Project.UnitOfWorks;


namespace Papara_Final_Project.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork, IValidator<CouponDTO> couponValidator)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CouponDTO>> GetAllCoupons()
        {
            var coupons = await _unitOfWork.Coupons.GetAllCoupons();
            return coupons.Select(c => new CouponDTO
            {
                Code = c.Code,
                DiscountAmount = c.DiscountAmount,
                ExpiryDate = c.ExpiryDate
            }).ToList();
        }

        public async Task<CouponDTO> GetCouponById(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetCouponById(id);
            if (coupon == null)
            {
                return null;
            }

            return new CouponDTO
            {
                Code = coupon.Code,
                DiscountAmount = coupon.DiscountAmount,
                ExpiryDate = coupon.ExpiryDate
            };
        }

        public async Task AddCoupon(CouponDTO couponDto)
        {

            var existingCoupon = await _unitOfWork.Coupons.GetCouponByCode(couponDto.Code);
            if (existingCoupon != null)
            {
                throw new ValidationException("Coupon code must be unique.");
            }

            var coupon = new Coupon
            {
                Code = couponDto.Code,
                DiscountAmount = couponDto.DiscountAmount,
                ExpiryDate = couponDto.ExpiryDate,
                IsUsed = false
            };

            await _unitOfWork.Coupons.AddCoupon(coupon);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCoupon(int id, CouponDTO couponDto)
        {

            var coupon = await _unitOfWork.Coupons.GetCouponById(id);
            if (coupon == null)
            {
                throw new KeyNotFoundException("Coupon not found");
            }

            var existingCoupon = await _unitOfWork.Coupons.GetCouponByCode(couponDto.Code);
            if (existingCoupon != null && existingCoupon.Id != id)
            {
                throw new ValidationException("Coupon code must be unique.");
            }

            coupon.Code = couponDto.Code;
            coupon.DiscountAmount = couponDto.DiscountAmount;
            coupon.ExpiryDate = couponDto.ExpiryDate;

            await _unitOfWork.Coupons.UpdateCoupon(coupon);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCoupon(int id)
        {
            await _unitOfWork.Coupons.DeleteCoupon(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
