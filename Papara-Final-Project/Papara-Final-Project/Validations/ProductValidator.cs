using FluentValidation;
using Papara_Final_Project.DTOs;

public class ProductValidator : AbstractValidator<ProductDTO>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("At least one category is required.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to zero.");
        RuleFor(x => x.RewardRate).InclusiveBetween(0, 20).WithMessage("Reward rate must be between 0 and 20.");
        RuleFor(x => x.MaxReward).GreaterThan(0).WithMessage("Max reward must be greater than zero.");
    }
}
