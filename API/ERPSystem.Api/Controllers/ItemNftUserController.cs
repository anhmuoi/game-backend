using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.ItemNftUser;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ItemNftUserController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IItemNftUserService _itemNftUserService;
    private readonly IMapper _mapper;

    public ItemNftUserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IItemNftUserService itemNftUserService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _itemNftUserService = itemNftUserService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiItemNftUsers)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult Gets(string search, int userId, List<int> status, bool getAll, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "Name",
        string sortDirection = "asc")
    {
        var itemNftUsers = _itemNftUserService.GetPaginated(search, status, userId, getAll, pageNumber, pageSize, sortColumn.ItemNftUserColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<ItemNftUserListModel>
        {
            Data = itemNftUsers,
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
    [Route(Constants.Route.ApiItemNftUsers)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Meeting)]
    public IActionResult Add([FromBody] ItemNftUserAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        int result = _itemNftUserService.AddItemNftUser(model);
        if (result != 0)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }
    /// <summary>
    /// Add new 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiAssignItemNftUsers)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Meeting)]
    public IActionResult AssignItemNftForUser(int idItemNftId, int userId)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _itemNftUserService.AssignItemNftForUser(idItemNftId,userId);
        if (!result)
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
    [Route(Constants.Route.ApiItemNftUsers)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }


        bool result = _itemNftUserService.DeleteMultiItemNftUsers(ids);
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
    [Route(Constants.Route.ApiItemNftUsersId)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult GetById(int id)
    {
        var item = _itemNftUserService.GetById(id);
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
    [Route(Constants.Route.ApiItemNftUsersId)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.Meeting)]
    public IActionResult Edit(int id, [FromBody] ItemNftUserEditModel model)
    {
        var item = _itemNftUserService.GetById(id);
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

   

        bool result = _itemNftUserService.UpdateItemNftUser(model);
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
    [Route(Constants.Route.ApiItemNftUsersId)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult Delete(int id)
    {
        var item = _itemNftUserService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
     
        bool result = _itemNftUserService.DeleteMultiItemNftUsers(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}