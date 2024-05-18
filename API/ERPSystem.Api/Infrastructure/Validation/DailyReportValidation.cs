using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel.DailyReport;
using ERPSystem.Service.Services;
using FluentValidation;

namespace ERPSystem.Api.Infrastructure.Validation;

public class DailyReportValidation : AbstractValidator<DailyReportModel>
{
    /// <summary>
    /// daily report validation
    /// </summary>
    /// <param name="dailyReportService"></param>
    public DailyReportValidation(IDailyReportService dailyReportService)
    {
        // RuleFor(reg => reg.Title).NotEmpty()
        //     .Must((reg, c) => !string.IsNullOrEmpty(reg.Title))
        //     .WithMessage(reg => string.Format(MessageResource.NotEmptyValidator, WorkLogResource.lblTitle));
        RuleFor(reg => reg.Date).NotEmpty()
            .Must((reg, c) => DateTimeHelper.IsDateTime(reg.Date))
            .WithMessage(string.Format(MessageResource.InvalidDateFormat, DailyReportResource.lblDateReport,
                Constants.Settings.DateTimeFormatDefault));
    }
}