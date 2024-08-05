using FluentValidation;
using Papara_Final_Project.DTOs;

public class CategoryValidator : AbstractValidator<CategoryDTO>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Category URL is required.");

        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Category tag is required.");
    }
}
