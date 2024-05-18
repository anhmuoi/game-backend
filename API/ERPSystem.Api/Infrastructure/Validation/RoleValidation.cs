using ERPSystem.Common.Resources;
using ERPSystem.DataModel.Role;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class RoleModelValidation : AbstractValidator<RoleModel>
{
    public RoleModelValidation(IRoleService roleService)
    {
        RuleFor(reg => reg.RoleName)
            .NotEmpty()
            .Must((reg, m) => !roleService.IsExistedName(m, reg.Id))
            .WithMessage(RoleResource.msgNameExisted);
    }
}