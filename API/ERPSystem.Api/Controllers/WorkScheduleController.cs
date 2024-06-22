using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Category;
using ERPSystem.DataModel.User;
using ERPSystem.DataModel.WorkSchedule;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WorkScheduleController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IWorkScheduleService _workScheduleService;
    private readonly IMapper _mapper;

    public WorkScheduleController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IWorkScheduleService workScheduleService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _workScheduleService = workScheduleService;
         _mapper = mapper;
    }
    /// <summary>
    /// Get init
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiWorkSchedulesInit)]
    public IActionResult GetInit()
    {
        return Ok(_workScheduleService.GetInit());
    }

    /// <summary>
    /// Get list work schedule
    /// </summary>
    /// <param name="search"></param>
    /// <param name="accounts"></param>
    /// <param name="date"></param>
    /// <param name="categoryTypes"></param>
    /// <param name="types"></param>
    /// <param name="categories"></param>
    /// <param name="folderLogs"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiWorkSchedules)]
    public IActionResult Gets(string search, string accounts, DateTime date, string categoryTypes, List<int> types, 
        List<int> categories, List<int> folderLogs, int pageNumber = 1, int pageSize = 10, string sortColumn = "Title", string sortDirection = "asc")
    {

        var workSchedules = _workScheduleService.GetPaginated(search, accounts, date, categoryTypes, types, categories, folderLogs, pageNumber, pageSize, sortColumn.ScheduleColumn(), sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<WorkScheduleListModel>
        {
            Data = workSchedules,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);

    }
    
    /// <summary>
    /// Add new schedule
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiWorkSchedules)]
    public IActionResult Add([FromBody] WorkScheduleModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _workScheduleService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi schedule
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiWorkSchedules)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }
        bool isUser = _workScheduleService.CheckCreatedBy(ids, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkScheduleResource.msgCanNotDeleteRecordOfAnotherUser);
        }
        bool result = _workScheduleService.DeleteMulti(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail schedule by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiWorkSchedulesId)]
    public IActionResult GetById(int id)
    {
        var item = _workScheduleService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        var schedule = _workScheduleService.CheckIsAllDay(item);
        return Ok(schedule);
    }

    /// <summary>
    /// Edit schedule by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiWorkSchedulesId)]
    public IActionResult Edit(int id, [FromBody] WorkScheduleModel model)
    {
        var item = _workScheduleService.GetById(id);
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
        bool isUser = _workScheduleService.CheckCreatedBy(new List<int>{id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkScheduleResource.msgCanNotEditRecordOfAnotherUser);
        }

        bool result = _workScheduleService.Update(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Delete schedule by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiWorkSchedulesId)]
    public IActionResult Delete(int id)
    {
        var item = _workScheduleService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        bool isUser = _workScheduleService.CheckCreatedBy(new List<int>{id}, _httpContext.User.GetAccountId());
        if (!isUser)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity,
                WorkScheduleResource.msgCanNotDeleteRecordOfAnotherUser);
        }
        bool result = _workScheduleService.Delete(id);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}