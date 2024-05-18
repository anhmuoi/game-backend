using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.WorkSchedule;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class WorkScheduleValidation : AbstractValidator<WorkScheduleModel>
{
    /// <summary>
    /// Work Schedule validation
    /// </summary>
    /// <param name="workScheduleService"></param>
    public WorkScheduleValidation(IWorkScheduleService workScheduleService)
    {
        RuleFor(reg => reg.StartDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, WorkScheduleResource.lblStartDate,
                Constants.Settings.DateTimeFormatDefault));
        RuleFor(reg => reg.EndDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, WorkScheduleResource.lblEndDate,
                Constants.Settings.DateTimeFormatDefault))
            .Must((reg, strings) => Helpers.CompareDate(reg.StartDate, reg.EndDate))
            .WithMessage(reg => string.Format(MessageResource.GreaterThan, WorkScheduleResource.lblEndDate, reg.StartDate));
        RuleFor(reg => reg.Type).Must((reg, c) => reg.Type >= 0)
            .WithMessage(reg => string.Format(MessageResource.NotSelected, WorkScheduleResource.lblWorkScheduleType));
    }
}