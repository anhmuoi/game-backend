using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.MeetingLog;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MeetingLogController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IMeetingLogService _meetingLogService;
    private readonly IMapper _mapper;

    public MeetingLogController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IMeetingLogService meetingLogService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _meetingLogService = meetingLogService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list meeting log
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="meetingRoomIds"></param>
    /// <param name="folderLogs"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiMeetingLogs)]
    public IActionResult Gets(int userId, List<int> meetingRoomIds, List<int> folderLogs, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "CreatedOn",
        string sortDirection = "asc")
    {
        var meetingLogs = _meetingLogService.GetPaginated(userId, meetingRoomIds, folderLogs, pageNumber, pageSize, sortColumn.MeetingLogColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<MeetingLogResponseModel>
        {
            Data = meetingLogs,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Add new meeting log
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiMeetingLogs)]
    public IActionResult Add([FromBody] MeetingLogModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _meetingLogService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi meeting log
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiMeetingLogs)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }

        // check current user map createdBy.
        bool isUser = _meetingLogService.CheckCreatedBy(ids, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                MeetingLogResource.msgCanNotDeleteRecordOfAnotherUser);
        }

        bool result = _meetingLogService.Delete(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail meeting log by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiMeetingLogsId)]
    public IActionResult GetById(int id)
    {
        var item = _meetingLogService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(item);
    }

    /// <summary>
    /// Edit meeting log by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiMeetingLogsId)]
    public IActionResult Edit(int id, [FromBody] MeetingLogModel model)
    {
        var item = _meetingLogService.GetById(id);
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

        bool isUser = _meetingLogService.CheckCreatedBy(new List<int>{id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                MeetingLogResource.msgCanNotEditRecordOfAnotherUser);
        }

        bool result = _meetingLogService.Update(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete meeting log by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiMeetingLogsId)]
    public IActionResult Delete(int id)
    {
        var item = _meetingLogService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        // check current user map createdBy.
        bool isUser = _meetingLogService.CheckCreatedBy(new List<int>(){id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                MeetingLogResource.msgCanNotDeleteRecordOfAnotherUser);
        }
        bool result = _meetingLogService.Delete(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}