using FluentValidation;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Validators.Category;

public class CategoryCreateValidator : AbstractValidator<CategoryCreateDTO>
{
    public CategoryCreateValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty()
            .WithMessage("Назва категорії обов'язкова")
            .MaximumLength(200)
            .WithMessage("Назва не може бути довшою за 200 символів");

        RuleFor(category => category.Slug)
            .NotEmpty()
            .WithMessage("Slug категорії обов'язковий")
            .MaximumLength(200)
            .WithMessage("Slug не може бути довшим за 200 символів");

        RuleFor(category => category.Url)
            .MaximumLength(500)
            .WithMessage("URL не може бути довшим за 500 символів")
            .When(category => !string.IsNullOrEmpty(category.Url));

        RuleFor(category => category.ParentId)
            .GreaterThan(0)
            .WithMessage("ParentId має бути більшим за 0")
            .When(category => category.ParentId.HasValue);
    }
}