using ERPSystem.Common.Resources;
using ERPSystem.DataModel.Category;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class CategoryAddModelValidation : AbstractValidator<CategoryAddModel>
{
    public CategoryAddModelValidation(ICategoryService categoryService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must(m => !categoryService.IsExistedName(m, 0))
            .WithMessage(CategoryResource.msgNameExisted);
        RuleFor(reg => reg.Color)
            .NotEmpty()
            .Must(m => !categoryService.IsExistedColor(m, 0))
            .WithMessage(CategoryResource.msgColorExisted);
    }
}

public class CategoryEditModelValidation : AbstractValidator<CategoryEditModel>
{
    public CategoryEditModelValidation(ICategoryService categoryService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !categoryService.IsExistedName(m, reg.Id))
            .WithMessage(CategoryResource.msgNameExisted);
        RuleFor(reg => reg.Color)
            .NotEmpty()
            .Must((reg, m) => !categoryService.IsExistedColor(m, reg.Id))
            .WithMessage(CategoryResource.msgColorExisted);
    }
}