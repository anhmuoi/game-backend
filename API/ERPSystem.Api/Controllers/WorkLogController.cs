using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.WorkLog;
using ERPSystem.DataModel.WorkSchedule;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WorkLogController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IWorkLogService _workLogService;
    private readonly IMapper _mapper;

    public WorkLogController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IWorkLogService workLogService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _workLogService = workLogService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list work log
    /// </summary>
    /// <param name="search"></param>
    /// <param name="userIds"></param>
    /// <param name="folderLogs"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiWorkLogs)]
    public IActionResult Gets(string search, List<int> userIds, List<int> folderLogs, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "Title",
        string sortDirection = "asc")
    {
        var workLogs = _workLogService.GetPaginated(search, userIds, folderLogs, pageNumber, pageSize, sortColumn.WorkLogColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<WorkLogResponseModel>
        {
            Data = workLogs,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Add new work log
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiWorkLogs)]
    public IActionResult Add([FromBody] WorkLogModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _workLogService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi work log
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiWorkLogs)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }

        // check current user map createdBy.
        bool isUser = _workLogService.CheckCreatedBy(ids, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkLogResource.msgCanNotDeleteRecordOfAnotherUser);
        }

        bool result = _workLogService.Delete(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail work log by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiWorkLogsId)]
    public IActionResult GetById(int id)
    {
        var item = _workLogService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(item);
    }

    /// <summary>
    /// Edit work log by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiWorkLogsId)]
    public IActionResult Edit(int id, [FromBody] WorkLogModel model)
    {
        var item = _workLogService.GetById(id);
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

        bool isUser = _workLogService.CheckCreatedBy(new List<int>{id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkLogResource.msgCanNotEditRecordOfAnotherUser);
        }

        bool result = _workLogService.Update(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete work log by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiWorkLogsId)]
    public IActionResult Delete(int id)
    {
        var item = _workLogService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        // check current user map createdBy.
        bool isUser = _workLogService.CheckCreatedBy(new List<int>(){id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkLogResource.msgCanNotDeleteRecordOfAnotherUser);
        }

        bool result = _workLogService.Delete(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}