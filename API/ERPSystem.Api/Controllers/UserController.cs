using System.Security.Claims;
using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Login;
using ERPSystem.DataModel.Response;
using ERPSystem.DataModel.User;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ERPSystem.Api.Controllers;

public class UserController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtHandler _jwtHandler;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IUserService userService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _jwtHandler = jwtHandler;
        _userService = userService;
         _mapper = mapper;
    }


    /// <summary>
    /// Get init
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiUsersInit)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetInit()
    {
        return Ok(_userService.GetInit());
    }

    /// <summary>
    /// Get list of users
    /// </summary>
    /// <param name="search"></param>
    /// <param name="status"></param>
    /// <param name="departmentIds"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiUsers)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Gets(string search, List<int> status, List<int> departmentIds, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name",
        string sortDirection = "desc")
    {

        var users = _userService.GetPaginated(search, status, departmentIds, pageNumber, pageSize, sortColumn.UserColumn(), sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<UserListModel>
        {
            Data = users,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };
        return Ok(pagingData);

    }

    /// <summary>
    /// Get detail user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiUsersId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetUserById(int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        var data = _mapper.Map<UserModel>(user);
        
        return Ok(data);
    }

    /// <summary>
    /// Add new user
    /// </summary>
    /// <param name="model">
    /// CreatedOn: Format MM.DD.YYYY HH:mm:ss
    /// </param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiUsers)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult AddUser([FromBody] UserAddModel model)
    {
        
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

         // Image Main
        if (model.Avatar.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            string path = $"{Constants.ImageConfig.BaseFolderUser}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Avatar, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Avatar = $"{hostApi}/static/{path}";
        }

        // Image Detail
        if (model.Avatar.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            string path = $"{Constants.ImageConfig.BaseFolderUser}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Avatar, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Avatar = $"{hostApi}/static/{path}";
        }

        int userId = _userService.AddUser(model);
        if (userId == 0)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        
        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess, JsonConvert.SerializeObject(new {id = userId}));
    }


    /// <summary>
    /// Update user by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model">
    /// UpdatedOn: Format MM.DD.YYYY HH:mm:ss
    /// </param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiUsersId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult UpdateUser(int id, [FromBody] UserEditModel model)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        
        model.Id = id;
        ModelState.Clear();
        if (!TryValidateModel(model))
        {
            return new ValidationFailedResult(ModelState);
        }

        // Image Main
        if (model.Avatar.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            
            // delete old link
            if (!string.IsNullOrEmpty(user.Avatar) && user.Avatar.Contains(hostApi))
            {
                FileHelpers.DeleteFileFromLink(user.Avatar.Replace($"{hostApi}/static/", ""));
            }
            
            // create new image
            string path = $"{Constants.ImageConfig.BaseFolderUser}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Avatar, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Avatar = $"{hostApi}/static/{path}";
        }

        
        bool isUpdate = _userService.UpdateUser(model);
        if (!isUpdate)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    /// <summary>
    /// Update user by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="groupId">
    /// UpdatedOn: Format MM.DD.YYYY HH:mm:ss
    /// </param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiUsersOutGroup)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult UserOutGroup(int id, int groupId)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
       

      
        
        bool isUpdate = _userService.UserOutGroup(id, groupId);
        if (!isUpdate)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiUsersId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult DeleteUser(int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        List<string> checkDepartmentManagers = _userService.CheckDepartmentManagerByUserId(new List<int>() { id });
        if (checkDepartmentManagers.Count > 0)
        {
            string msgError = UserResource.msgCanNotDeleteDepartmentManager + $" ({checkDepartmentManagers.First()})";
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, msgError);
        }
        
        bool isDelete = _userService.DeleteMultiUsers(new List<int>() { id });
        if (!isDelete)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Delete multi users
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiUsers)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult DeleteMultiUsers(List<int>? ids)
    {
        if (ids != null && !ids.Any())
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        List<string> checkDepartmentManagers = _userService.CheckDepartmentManagerByUserId(ids);
        if (checkDepartmentManagers.Count > 0)
        {
            string msgError = UserResource.msgCanNotDeleteDepartmentManager + $" ({checkDepartmentManagers.First()})";
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, msgError);
        }
        
        bool isDelete = _userService.DeleteMultiUsers(ids);
        if (!isDelete)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }


    /// <summary>
    /// Get Account profile information
    /// </summary>
    /// <returns></returns>
    /// <response code="401">Unauthorized: Token not invalid</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiAccountProfile)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetProfile()
    {
        var model = new UserModel();
        int id = _httpContext.User.GetAccountId();

        if (id != 0)
        {
            var account = _userService.GetUserByAccountId(id);
            if (account == null )
                return new ApiErrorResult(StatusCodes.Status404NotFound);

            model =_mapper.Map<UserModel>(account);
            model.Id = id;
        }

        return Ok(model);
    }

    /// <summary>
    /// Edit the account profile information.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="422">Unprocessable Entity: Id or data model wrong</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPut]
    [Route(Constants.Route.ApiAccountProfile)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult EditProfile([FromBody] AccountEditModel model)
    {
        model.Id = _httpContext.User.GetAccountId();
      
        
        ModelState.Clear();
       

        _userService.UpdateProfile(model);

        return new ApiSuccessResult(StatusCodes.Status200OK,
            string.Format(MessageResource.MessageUpdateSuccess, AccountResource.lblAccount, ""));
    }



    /// <summary>
    /// Update image for user
    /// </summary>
    /// <param name="model">Object model json include avatar</param>
    /// <returns></returns>
    /// <response code="401">Unauthorized: Token not invalid</response>
    /// <response code="400">Bad Request: Avatar not string.Empty</response>
    /// <response code="404">Not Found: Id not exist in DB</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPut]
    [Route(Constants.Route.ApiAccountsAvatar)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult UploadAvatar([FromBody] UserAvatarModel model)
    {
        var user = _userService.GetUserByAccountId(_httpContext.User.GetAccountId());
        if (user == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, AccountResource.msgInvalidAccount);
        }

        
        if (string.IsNullOrEmpty(model.Avatar))
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, AccountResource.msgEmptyAvatar);
        }

        // avatar
        if (model.Avatar.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            
            // delete old link
            if (!string.IsNullOrEmpty(user.Avatar) && user.Avatar.Contains(hostApi))
            {
                FileHelpers.DeleteFileFromLink(user.Avatar.Replace($"{hostApi}/static/", ""));
            }
            
            // create new image
            string path = $"{Constants.ImageConfig.BaseFolderUser}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Avatar, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Avatar = $"{hostApi}/static/{path}";
        }

        _userService.UpdateAvatar(user.Id, model.Avatar);

        
        return new ApiSuccessResult(StatusCodes.Status200OK,
            string.Format(MessageResource.MessageUpdateSuccess, AccountResource.lblAccount, ""));
    }

    /// <summary>
    /// Get avatar for user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiAccountsAvatar)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetAvatar()
    {
        var user = _userService.GetUserByAccountId(_httpContext.User.GetAccountId());
        if (user == null) return new ApiErrorResult(StatusCodes.Status404NotFound, AccountResource.msgInvalidUser);
        var model = new UserAvatarModel();
        model.Avatar = user.Avatar;

        return Ok(model);
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="model">JSON model for object(new password, confirm new password and token string)</param>
    /// <returns></returns>
    /// <response code="400">Bad Request: Email does not exist or wrong token</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiResetPassword)]
    public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        var clAcc = _jwtHandler.GetPrincipalFromExpiredToken(model.Token);
        if (clAcc == null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.InvalidInformation);
        }

        var username = clAcc.GetUsername();
        var account = _userService.GetAccountByUserName(username);
        if (account == null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.EmailDoestNotExist);
        }

        account.Password = SecurePasswordHasher.Hash(model.NewPassword);
        account.UpdatedOn = DateTime.UtcNow;

        _userService.ChangePassword(account);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.MessageResetPassSuccess);
    }

    /// <summary>
    /// When forgot password
    /// </summary>
    /// <param name="model">JSON model for string of email login</param>
    /// <returns></returns>
    /// <response code="400">Bad Request: Email does not exist in DB</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiForgotPassword)]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        var account = _userService.GetAccountByEmail(model.Email);
        if (account == null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.MessageEmailNotRegister);
        }

        _userService.SendResetAccountMail(account, model.Email);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.MessageSendEmailSuccess);
    }
}