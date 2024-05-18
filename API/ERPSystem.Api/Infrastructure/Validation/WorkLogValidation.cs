using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.WorkLog;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class WorkLogValidation : AbstractValidator<WorkLogModel>
{
    /// <summary>
    /// Work Log validation
    /// </summary>
    /// <param name="workLogService"></param>
    public WorkLogValidation(IWorkLogService workLogService)
    {
        RuleFor(reg => reg.StartDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, WorkLogResource.lblStartDate,
                Constants.Settings.DateTimeFormatDefault));
        RuleFor(reg => reg.EndDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, WorkLogResource.lblEndDate,
                Constants.Settings.DateTimeFormatDefault))
            .Must((reg, strings) => Helpers.CompareDate(reg.StartDate, reg.EndDate))
            .WithMessage(reg => string.Format(MessageResource.GreaterThan, WorkLogResource.lblEndDate, reg.StartDate));
        RuleFor(reg => reg.Title).NotEmpty()
            .Must((reg, c) => !string.IsNullOrEmpty(reg.Title))
            .WithMessage(reg => string.Format(MessageResource.NotEmptyValidator, WorkLogResource.lblTitle));
    }
}