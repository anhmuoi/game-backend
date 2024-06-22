using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Role;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RoleController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtHandler _jwtHandler;
    private readonly IRoleService _roleService;
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public RoleController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler, IRoleService roleService, IAccountService accountService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _jwtHandler = jwtHandler;
        _roleService = roleService;
        _accountService = accountService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list of Role from DB
    /// </summary>
    /// <param name="search">Query string that filter by name</param>
    /// <param name="pageNumber">Page number start from 1</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="sortColumn">Sort Column by string name of the column</param>
    /// <param name="sortDirection">Sort direction: ‘desc’ for descending , ‘asc’ for ascending</param>
    /// <returns></returns>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiRoles)]
    public IActionResult Gets(string search, int pageNumber = 1, int pageSize = 10, string sortColumn = "0", string sortDirection = "desc")
    {
        sortColumn = Helpers.CheckPropertyInObject<RoleModel>(sortColumn, "RoleName");
        var permissions = _roleService
            .GetPaginated(search, pageNumber, pageSize, sortColumn, sortDirection, out var recordsTotal,
                out var recordsFiltered).AsEnumerable().ToList();

        var pagingData = new PagingData<RoleModel>
        {
            Data = permissions,
            Meta = { RecordsTotal = recordsTotal, RecordsFiltered = recordsFiltered }
        };

        return Ok(pagingData);
    }


    /// <summary>
    /// Get role information by id
    /// </summary>
    /// <param name="id"> identifier of role </param>
    /// <returns></returns>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="404">Not Found: Role does not exist in DB</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiRolesId)]
    public IActionResult Get(int id)
    {
        var model = new RoleModel();
        var role = new Role();
        if (id != 0)
        {
            role = _roleService.GetById(id);
            if (role == null)
            {
                return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.msgNotFoundRole);
            }

            // Should compare below two list.
            // To print out the list of permissions created after the role creation.
            // If it is a permission in the DB, print it out as the value stored in the DB.
            // But the permission that is created after the role creation, print it out the value as "false".
            var defaultPermissionGroupList = _roleService.GetDefaultRoleValue();
            var userPermissionGroups = _roleService.ChangeJsonToModel(role.PermissionList);

            foreach (var defaultGroupModel in defaultPermissionGroupList)
            {
                var userPermissionGroup =
                    userPermissionGroups.FirstOrDefault(m => m.Title == defaultGroupModel.Title);

                if (userPermissionGroup != null)
                {
                    foreach (var defaultModel in defaultGroupModel.Permissions)
                    {
                        var userPermission = userPermissionGroup.Permissions
                            .FirstOrDefault(m => m.Title == defaultModel.Title);

                        defaultModel.IsEnabled = userPermission != null ? userPermission.IsEnabled : false;
                    }
                }
                else
                {
                    foreach (var defaultModel in defaultGroupModel.Permissions)
                    {
                        defaultModel.IsEnabled = false;
                    }
                }
            }

            model.Id = role.Id;
            model.RoleName = role.Name;
            model.IsDefault = role.IsDefault;
            model.PermissionGroups = defaultPermissionGroupList;
        }
        else
        {
            model.PermissionGroups = _roleService.GetDefaultRoleValue();
        }

        model.PermissionGroups = model.PermissionGroups.OrderBy(m => m.Title).ToList();
        return Ok(model);
    }

    /// <summary>
    /// Add a new role to system
    /// </summary>
    /// <param name="model"> This model has information about the device to be added. </param>
    /// <param name="similarId"> identifier of role that is similar-made </param>
    /// <returns></returns>
    /// <response code="201">Create new a role</response>
    /// <response code="400">Bad Request: similar role not null</response>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPost]
    [Route(Constants.Route.ApiRoles)]
    public IActionResult Add([FromBody] RoleModel model, int similarId = 0)
    {
        // Below code is for unexcepted exception.
        // There is an error about edit -> add in very quick.
        // In some cases, API can't check the duplication because the Id value in model is sent as same with edited data's Id from FE.
        // ex) Id = 108 data is edited, and new role is added, Add model's Id value is 108.
        ModelState.Clear();
        model.Id = 0;

        if (!TryValidateModel(model))
        {
            return new ValidationFailedResult(ModelState);
        }

        if (similarId != 0)
        {
            var similarRole = _roleService.GetById(similarId);

            if (similarRole != null)
            {
                model.PermissionGroups = _roleService.ChangeJsonToModel(similarRole.PermissionList);
            }
            else
                return new ApiErrorResult(StatusCodes.Status400BadRequest,
                    string.Format(MessageResource.AnUnexpectedErrorOccurred));
        }

        if (!_roleService.CheckPermissions(model.PermissionGroups))
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest,
                string.Format(MessageResource.msgDuplicatedData, RoleResource.lblRole));
        }

        _roleService.Add(model);

        return new ApiSuccessResult(StatusCodes.Status201Created,
            string.Format(MessageResource.MessageAddSuccess, RoleResource.lblRole));
    }


    /// <summary>
    /// Update permission list of spectific role
    /// </summary>
    /// <param name="id"> identifier of this role </param>
    /// <param name="model"> data model that include information </param>
    /// <param name="similarId"> identifier of role that is similar-made </param>
    /// <returns></returns>
    /// <response code="400">Bad Request: similar role not null</response>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPut]
    [Route(Constants.Route.ApiRolesId)]
    public IActionResult Edit(int id, [FromBody] RoleModel model, int similarId = 0)
    {
        model.Id = id;
        ModelState.Clear();
        if (!TryValidateModel(model))
        {
            return new ValidationFailedResult(ModelState);
        }

        if (similarId != 0)
        {
            var similarRole = _roleService.GetById(similarId);

            if (similarRole != null)
                model.PermissionGroups = _roleService.ChangeJsonToModel(similarRole.PermissionList);
            else
                return new ApiErrorResult(StatusCodes.Status400BadRequest,
                    string.Format(MessageResource.AnUnexpectedErrorOccurred));
        }

        if (model.PermissionGroups != null && model.PermissionGroups.Any() &&
            !_roleService.CheckPermissions(model.PermissionGroups))
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest,
                string.Format(MessageResource.msgDuplicatedData, RoleResource.lblRole));
        }

        var role = _roleService.GetById(id);

        if (role != null)
        {
            model.Id = id;

            if (role.Type != (int)RoleType.DynamicRole && model.PermissionGroups != null)
            {
                return new ApiErrorResult(StatusCodes.Status400BadRequest,
                    string.Format(MessageResource.InvalidRoleUpdate));
            }

            _roleService.Update(model);

            return new ApiSuccessResult(StatusCodes.Status200OK,
                string.Format(MessageResource.MessageUpdateSuccess, role.Name, $"({RoleResource.lblRole})"));
        }

        return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.msgNotFoundRole);
    }


    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id"> identifier of dynamic role </param>
    /// <returns></returns>
    /// <response code="400">Bad Request: type role in role not null</response>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="404">Not Found: Role does not exist in DB</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpDelete]
    [Route(Constants.Route.ApiRolesId)]
    public IActionResult Delete(int id)
    {
        var role = _roleService.GetById(id);
        if (role == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.msgNotFoundRole);
        }

        if (role.Type != (int)RoleType.DynamicRole)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest,
                string.Format(MessageResource.msgFailedDeleteBasicRole));
        }

        var accounts = _accountService.GetAccountByRoleId(role.Id);
        if (accounts != null && accounts.Any())
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest,
                string.Format(MessageResource.msgFailedDeleteRole));
        }

        _roleService.Delete(role);

        return new ApiSuccessResult(StatusCodes.Status200OK,
            string.Format(MessageResource.MessageDeleteSuccess, RoleResource.lblRole));
    }


    /// <summary>
    /// Delete multiple roles
    /// </summary>
    /// <param name="ids"> list of identifier of dynamic role </param>
    /// <returns></returns>
    /// <response code="400">Bad Request: type role in role not null</response>
    /// <response code="401">Unauthorized: Lack of String Token Bearer</response>
    /// <response code="403">Forbidden: Login with other account with role higher</response>
    /// <response code="404">Not Found: List of Roles does not exist in DB</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpDelete]
    [Route(Constants.Route.ApiRoles)]
    public IActionResult DeleteMultiple(List<int> ids)
    {

        var roles = _roleService.GetByIds(ids);
        if (!roles.Any())
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.msgNotFoundRole);
        }

        foreach (var role in roles)
        {
            if (role.Type != (int)RoleType.DynamicRole)
            {
                return new ApiErrorResult(StatusCodes.Status400BadRequest,
                    string.Format(MessageResource.msgFailedDeleteBasicRole));
            }
        }

        var accounts = _accountService.GetAccountByRoleIds(ids);
        if (accounts != null && accounts.Any())
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest,
                string.Format(MessageResource.msgFailedDeleteRole));
        }

        _roleService.DeleteMultiple(roles);

        return new ApiSuccessResult(StatusCodes.Status200OK,
            string.Format(MessageResource.MessageDeleteSuccess, RoleResource.lblRole));
    }


    /// <summary>
    /// Update role setting default
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiChangeRoleSettingDefault)]
    public IActionResult ChangeDefaultRoleSettingForUser(int id)
    {
        var role = _roleService.GetById(id);
        if (role == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.msgNotFoundRole);
        }            
        _roleService.ChangeDefaultSettingRole(id);
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.MessageSuccess);
    }

    /// <summary>
    /// Check my permission with action name
    /// </summary>
    /// <param name="actionName">permission name</param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiRolesMyPermission)]
    public IActionResult CheckMyPermission(string actionName)
    {
        if (string.IsNullOrEmpty(actionName))
            return Ok(new { data = false });
        
        return Ok(new { data = _roleService.CheckPermissionEnabled(actionName, _httpContext.User.GetAccountId()) });
    }
}