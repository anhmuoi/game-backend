using ERPSystem.Common.Resources;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class FolderLogValidation : AbstractValidator<FolderLogModel>
{
    public FolderLogValidation(IFolderLogService folderLogService)
    {
        RuleFor(reg => reg.Name)
            .NotEmpty()
            .Must((reg, m) => !folderLogService.IsExistedNameInLevel(m, reg.ParentId, reg.Id))
            .WithMessage(DriverResource.msgFolderNameExisted);
    }
}