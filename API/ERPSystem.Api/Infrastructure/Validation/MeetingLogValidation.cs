using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.MeetingLog;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class MeetingLogValidation : AbstractValidator<MeetingLogModel>
{
    /// <summary>
    /// Work Log validation
    /// </summary>
    /// <param name="meetingLogService"></param>
    public MeetingLogValidation(IMeetingLogService meetingLogService)
    {
        RuleFor(reg => reg.StartDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, MeetingLogResource.lblStartDate,
                Constants.Settings.DateTimeFormatDefault));
        RuleFor(reg => reg.EndDate).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.StartDate))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, MeetingLogResource.lblEndDate,
                Constants.Settings.DateTimeFormatDefault))
            .Must((reg, strings) => Helpers.CompareDate(reg.StartDate, reg.EndDate))
            .WithMessage(reg => string.Format(MessageResource.GreaterThan, MeetingLogResource.lblEndDate, reg.StartDate));
        RuleFor(reg => reg.Title).NotEmpty()
            .Must((reg, c) => !string.IsNullOrEmpty(reg.Title))
            .WithMessage(reg => string.Format(MessageResource.NotEmptyValidator, MeetingLogResource.lblTitle));
    }
}