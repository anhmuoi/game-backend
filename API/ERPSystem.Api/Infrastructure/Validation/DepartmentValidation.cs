using ERPSystem.Common.Resources;
using ERPSystem.DataModel.Department;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class DepartmentAddModelValidation : AbstractValidator<DepartmentAddModel>
{
    public DepartmentAddModelValidation(IDepartmentService departmentService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must(m => !departmentService.IsExistedName(m, 0))
            .WithMessage(DepartmentResource.msgNameExisted);
        
        RuleFor(reg => reg.Number)
            .NotEmpty()
            .Must(m => !departmentService.IsExistedNumber(m, 0))
            .WithMessage(DepartmentResource.msgNumberExisted);
    }
}

public class DepartmentEditModelValidation : AbstractValidator<DepartmentEditModel>
{
    public DepartmentEditModelValidation(IDepartmentService departmentService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !departmentService.IsExistedName(m, reg.Id))
            .WithMessage(DepartmentResource.msgNameExisted);
        
        RuleFor(reg => reg.Number)
            .NotEmpty()
            .Must((reg, m) => !departmentService.IsExistedNumber(m, reg.Id))
            .WithMessage(DepartmentResource.msgNumberExisted);
    }
}