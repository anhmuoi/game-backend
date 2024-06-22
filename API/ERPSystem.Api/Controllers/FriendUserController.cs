using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.FriendUser;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;


public class FriendUserController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IFriendUserService _friendUserService;
    private readonly IJwtHandler _jwtHandler;
    private readonly IMapper _mapper;

    public FriendUserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IFriendUserService friendUserService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _friendUserService = friendUserService;
        _mapper = mapper;
        _jwtHandler = jwtHandler;
    }

    /// <summary>
    /// Get list 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsers)]
    public IActionResult Gets(int userId, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "CreatedOn",
        string sortDirection = "asc")
    {
        var friendUsers = _friendUserService.GetPaginated(userId, pageNumber, pageSize, sortColumn.FriendUserColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<FriendUserListModel>
        {
            Data = friendUsers,
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsers)]
    public IActionResult Add([FromBody] FriendUserAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }
       

        int result = _friendUserService.AddFriendUser(model);
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsers)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }


        bool result = _friendUserService.DeleteMultiFriendUsers(ids);
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsersId)]
    public IActionResult GetById(int id)
    {
        var item = _friendUserService.GetById(id);
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsersId)]
    public IActionResult Edit(int id, [FromBody] FriendUserEditModel model)
    {
        var item = _friendUserService.GetById(id);
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

   

        bool result = _friendUserService.UpdateFriendUser(model);
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(Constants.Route.ApiFriendUsersId)]
    public IActionResult Delete(int id)
    {
        var item = _friendUserService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
     
        bool result = _friendUserService.DeleteMultiFriendUsers(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }


    /// <summary>
    /// send request add friend 
    /// </summary>
    /// <param name="userId1"></param>
    /// <param name="userId2"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFriendsIdAddFriend)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult RequestAddFriend(int userId1, int userId2)
    {
        
        bool result = _friendUserService.RequestAddFriend(userId1, userId2);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }


    /// <summary>
    /// confirm request add friend 
    /// </summary>
    /// <param name="userId1"></param>
    /// <param name="userId2"></param>
    /// <param name="token"></param>
    /// <param name="confirm"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiFriendsIdConfirmAddFriend)]
    public IActionResult ConfirmAddFriend(int userId1, int userId2, bool confirm, string token)
    {

        var clAcc = _jwtHandler.GetPrincipalFromExpiredToken(token);
        if (clAcc == null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.InvalidInformation);
        }
       

        bool result = _friendUserService.ConfirmAddFriend(userId1, userId2, confirm);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
}