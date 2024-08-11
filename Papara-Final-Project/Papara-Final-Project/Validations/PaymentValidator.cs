using FluentValidation;
using Papara_Final_Project.DTOs;

public class PaymentValidator : AbstractValidator<PaymentDTO>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.CardNumber).Length(16).WithMessage("Card length must be 16 digits long.");
        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Expiry date is required.")
            .Matches(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$").WithMessage("Invalid expiry date format.");
        RuleFor(x => x.CVV)
            .NotEmpty().WithMessage("CVV is required.")
            .Length(3).WithMessage("CVV must be 3 digits long.");
    }
}
