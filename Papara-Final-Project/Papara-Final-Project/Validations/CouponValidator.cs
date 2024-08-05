using FluentValidation;
using Papara_Final_Project.DTOs;

namespace Papara_Final_Project.Validations
{
    public class CouponValidator : AbstractValidator<CouponDTO>
    {
        public CouponValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(1, 10).WithMessage("Code must be between 1 and 10 characters.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThan(0).WithMessage("Discount amount must be greater than zero.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.Now).WithMessage("Expiry date must be in the future.");
        }
    }
}
