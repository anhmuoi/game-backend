using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Department;
using ERPSystem.DataModel.User;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;


public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly IMapper _mapper;
    
    private readonly IJwtHandler _jwtHandler;

    public DepartmentController(IDepartmentService departmentService, IMapper mapper,  IJwtHandler jwtHandler)
    {
        _departmentService = departmentService;
        _jwtHandler = jwtHandler;
        _mapper = mapper;
    }

    /// <summary>
    /// Get List department
    /// </summary>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="types"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDepartments)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Department)]
    public IActionResult Gets(string name, string number, List<int>types, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var filter = new DepartmentFilterModel()
        {
            Name = name,
            Number = number,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.DepartmentColumn(),
            SortDirection = sortDirection,
        };
        var data = _departmentService.Gets(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<DepartmentListModel>()
        {
            Data = data,
            Meta = new Meta()
            {
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
            }
        };
        
        return Ok(pagingData);
    }
    
    /// <summary>
    /// Add new department
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiDepartments)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Department)]
    public IActionResult Add([FromBody] DepartmentAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _departmentService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }
    
    /// <summary>
    /// Delete multi department
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiDepartments)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Department)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }
        
        // Check department has been used
        bool isUsedDepartment = _departmentService.CheckDepentdance(ids);
        if (isUsedDepartment)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, DepartmentResource.msgDeleteFalseDepartmentDependant);
        }
        
        bool result = _departmentService.Delete(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get detail department by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDepartmentsId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Department)]
    public IActionResult GetById(int id)
    {
        var item = _departmentService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(_mapper.Map<DepartmentModel>(item));
    }

    /// <summary>
    /// Edit department by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiDepartmentsId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.Department)]
    public IActionResult Edit(int id, [FromBody] DepartmentEditModel model)
    {
        var item = _departmentService.GetById(id);
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

        bool result = _departmentService.Edit(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Delete department by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiDepartmentsId)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Department)]
    public IActionResult Delete(int id)
    {
        var item = _departmentService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _departmentService.Delete(new List<int>() { id });
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Get department tree
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDepartmentsTree)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Department)]
    public IActionResult GetDepartmentTree()
   {
        return Ok(_departmentService.GetDepartmentTree());
    }

    /// <summary>
    /// Get users assigned by department id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDepartmentsIdAssign)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.User)]
    public IActionResult GetUsersByDepartment(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var filter = new UserFilterModel()
        {
            Name = name,
            DepartmentIds = new List<int>() { id },
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.DepartmentColumn(),
            SortDirection = sortDirection,
        };
        var data = _departmentService.GetUsersByDepartmentId(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserListModel>()
        {
            Data = data,
            Meta = new Meta()
            {
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
            }
        };
        
        return Ok(pagingData);
    }
    
    /// <summary>
    /// Assign users to department
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiDepartmentsIdAssign)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.User)]
    public IActionResult AssignUsersToDepartment(int id, [FromBody]List<int> userIds)
    {
        var item = _departmentService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _departmentService.AssignUsersToDepartment(id, userIds);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// send request join group
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiDepartmentsIdJoinGroup)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.User)]
    public IActionResult RequestJoinGroup(int id, int userId)
    {
        var item = _departmentService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _departmentService.RequestJoinGroup(id, userId);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }


    /// <summary>
    /// send request join group
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="confirm"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiDepartmentsIdConfirmJoinGroup)]
    public IActionResult ConfirmJoinGroup(int id, int userId, bool confirm, string token)
    {

        var clAcc = _jwtHandler.GetPrincipalFromExpiredToken(token);
        if (clAcc == null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.InvalidInformation);
        }
        var item = _departmentService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _departmentService.ConfirmJoinGroup(id, userId, confirm);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Get users un-assign to department
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDepartmentsIdUnAssign)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.User)]
    public IActionResult GetUsersWithoutDepartment(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var filter = new UserFilterModel()
        {
            Name = name,
            DepartmentIds = new List<int>() { id },
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.DepartmentColumn(),
            SortDirection = sortDirection,
        };
        var data = _departmentService.GetUsersWithoutDepartmentId(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserListModel>()
        {
            Data = data,
            Meta = new Meta()
            {
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
            }
        };
        
        return Ok(pagingData);
    }
}