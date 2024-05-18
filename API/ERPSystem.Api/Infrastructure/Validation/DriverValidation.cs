using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.Driver;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class FolderAddModelValidation : AbstractValidator<FolderAddModel>
{
    public FolderAddModelValidation(IDriverService driverService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !driverService.IsExistedNameInLevel(m, reg.ParentId, 0))
            .WithMessage(DriverResource.msgFolderNameExisted);
        
        RuleFor(reg => reg.ParentId)
            .NotNull()
            .Must(driverService.CanBeEditFolder)
            .WithMessage(DriverResource.msgNotPermissionInFolder);
    }
}

public class DriverShareModelValidation : AbstractValidator<DriverShareModel>
{
    public DriverShareModelValidation()
    {
        RuleFor(reg => reg.UserIds)
            .NotNull()
            .Must(m => m.Count > 0)
            .WithMessage(MessageResource.BadRequest);
        
        RuleFor(reg => reg.PermissionId)
            .Must(m => EnumHelper.ToEnumList<MediaPermission>().Where(n => n.Id != (int)MediaPermission.Owner).Select(n => n.Id).Contains(m))
            .WithMessage(MessageResource.BadRequest);
    }
}