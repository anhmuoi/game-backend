using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.DailyReport;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DailyReportController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IDailyReportService _dailyReportService;
    private readonly IMapper _mapper;

    public DailyReportController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IDailyReportService dailyReportService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _dailyReportService = dailyReportService;
        _mapper = mapper;
    }
    /// <summary>
    /// Get init
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiDailyReportsInit)]
    public IActionResult GetInit()
    {
        return Ok(_dailyReportService.GetInit());
    }
    /// <summary>
    /// get list daily report
    /// </summary>
    /// <param name="search"></param>
    /// <param name="userIds"></param>
    /// <param name="folderLogs"></param>
    /// <param name="reporters"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiDailyReports)]
    public IActionResult Gets(string search, List<int> userIds, List<int> folderLogs, List<int> reporters,List<int> departmentIds , DateTime start, DateTime end, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "userId",
        string sortDirection = "asc")
    {
        var workLogs = _dailyReportService.GetPaginated(search, userIds, folderLogs, reporters, departmentIds, start, end, pageNumber, pageSize, sortColumn.DailyReportColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<DailyReportResponseModel>
        {
            Data = workLogs,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Add new daily report
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiDailyReports)]
    public IActionResult Add([FromBody] DailyReportModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _dailyReportService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi daily report
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiDailyReports)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }

        // check current user map createdBy.
        bool isUser = _dailyReportService.CheckCreatedBy(ids, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                DailyReportResource.msgCanNotDeleteRecordOfAnotherUser);
        }

        bool result = _dailyReportService.Delete(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail daily report by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDailyReportsId)]
    public IActionResult GetById(int id)
    {
        var item = _dailyReportService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(item);
    }
    /// <summary>
    /// Get detail daily report by userId and date
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDailyReportsByUserIdAndDate)]
    public IActionResult GetByUserIdAndDate(int userId, string date)
    {
        var item = _dailyReportService.GetByUserIdAndDate(userId, date);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(item);
    }

    /// <summary>
    /// Edit daily report by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiDailyReportsId)]
    public IActionResult Edit(int id, [FromBody] DailyReportModel model)
    {
        var item = _dailyReportService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        model.Id = id;
        ModelState.Clear();
        if (!TryValidateModel(model))
        {
            return new ValidationFailedResult(ModelState);
        }

        bool isUser = _dailyReportService.CheckCreatedBy(new List<int>{id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                DailyReportResource.msgCanNotEditRecordOfAnotherUser);
        }

        bool result = _dailyReportService.Update(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete daily report by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiDailyReportsId)]
    public IActionResult Delete(int id)
    {
        var item = _dailyReportService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _dailyReportService.Delete(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}