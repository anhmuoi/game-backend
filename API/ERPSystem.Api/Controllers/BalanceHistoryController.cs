using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.BalanceHistory;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BalanceHistoryController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IBalanceHistoryService _balanceHistoryService;
    private readonly IMapper _mapper;

    public BalanceHistoryController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IBalanceHistoryService balanceHistoryService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _balanceHistoryService = balanceHistoryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiBalanceHistorys)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult Gets(string search, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "Name",
        string sortDirection = "asc")
    {
        var balanceHistorys = _balanceHistoryService.GetPaginated(search, pageNumber, pageSize, sortColumn.BalanceHistoryColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<BalanceHistoryListModel>
        {
            Data = balanceHistorys,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Add new 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiBalanceHistorys)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Meeting)]
    public IActionResult Add([FromBody] BalanceHistoryAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }
       

        int result = _balanceHistoryService.AddBalanceHistory(model);
        if (result != 0)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi 
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiBalanceHistorys)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }


        bool result = _balanceHistoryService.DeleteMultiBalanceHistorys(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail  by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiBalanceHistorysId)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult GetById(int id)
    {
        var item = _balanceHistoryService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(item);
    }

    /// <summary>
    /// Edit  by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiBalanceHistorysId)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.Meeting)]
    public IActionResult Edit(int id, [FromBody] BalanceHistoryEditModel model)
    {
        var item = _balanceHistoryService.GetById(id);
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

   

        bool result = _balanceHistoryService.UpdateBalanceHistory(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete  by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiBalanceHistorysId)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult Delete(int id)
    {
        var item = _balanceHistoryService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
     
        bool result = _balanceHistoryService.DeleteMultiBalanceHistorys(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}