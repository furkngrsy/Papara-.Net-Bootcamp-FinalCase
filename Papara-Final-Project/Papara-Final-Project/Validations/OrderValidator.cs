using FluentValidation;
using Papara_Final_Project.DTOs;

namespace Papara_Final_Project.Validations
{
    public class OrderValidator : AbstractValidator<OrderDTO>
    {
        public OrderValidator()
        {
            RuleFor(o => o.OrderDetails)
                .NotEmpty().WithMessage("Order must have at least one product.")
                .Must(od => od.All(d => d.Quantity > 0)).WithMessage("Quantity of each product must be greater than zero.");

            RuleFor(o => o.CouponCode)
                .Length(0, 10).WithMessage("Coupon code must be up to 10 characters long.");
        }
    }
}
