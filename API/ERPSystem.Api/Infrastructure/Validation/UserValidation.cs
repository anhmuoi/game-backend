using FluentValidation;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.User;
using ERPSystem.Service;
using ERPSystem.Service.Services;

namespace ERPSystem.Api.Infrastructure.Validation;

public class UserModelValidation : AbstractValidator<UserAddModel>
{
    public UserModelValidation(IUserService userService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !userService.IsExistedUserName(m, reg.Id))
            .WithMessage(UserResource.msgUserNameExisted);
        RuleFor(reg => reg.UserName)
            .NotEmpty()
            .Must((reg, m) => !userService.IsExistedEmail(m, reg.Id))
            .WithMessage(UserResource.msgUserEmailExisted);
        
        // RuleFor(reg => reg.Phone)
        //     .NotEmpty()
        //     .Must((reg, m) => !userService.IsExistedUserPhone(m, reg.Id))
        //     .WithMessage(UserResource.msgUserPhoneExisted);
        
    }
}
public class AddUserModelValidation : AbstractValidator<UserEditModel>
{
    public AddUserModelValidation(IUserService userService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !userService.IsExistedUserName(m, reg.Id))
            .WithMessage(UserResource.msgUserNameExisted);
        RuleFor(reg => reg.UserName)
            .NotEmpty()
            .Must((reg, m) => !userService.IsExistedEmail(m, reg.Id))
            .WithMessage(UserResource.msgUserEmailExisted);
        
    }
}