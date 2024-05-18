using ERPSystem.Common.Infrastructure;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IWorkScheduleService _workScheduleService;
    private readonly IMeetingLogService _meetingService;

    public DashboardController(IHttpContextAccessor contextAccessor, IWorkScheduleService workScheduleService, IMeetingLogService meetingService)
    {
        _httpContext = contextAccessor.HttpContext;
        _workScheduleService = workScheduleService;
        _meetingService = meetingService;
    }

    [HttpGet]
    [Route(Constants.Route.ApiDashboard)]
    public IActionResult Dashboard()
    {
        int accountId = _httpContext.User.GetAccountId();
        string timezone = _httpContext.User.GetTimezone();

        // DateTime now = DateTimeHelper.ConvertDateTimeUTCToSystemTimeZone(timezone);
        DateTime now = DateTime.UtcNow;
        // get list meeting log of 3 day.
        var listMetting = _meetingService.GetWorkLogDashboards(now);
        // get list work schedule of 3 day.
        var listSchedule = _workScheduleService.GetScheduleDashboards(now);

        DashboardModel dashboardModel = new DashboardModel
        {
            TotalMetting = listMetting.Count,
            TotalSchedule = listSchedule.Count,
            Meetings = listMetting,
            Schedules = listSchedule
        };

        return Ok(dashboardModel);
    }
}