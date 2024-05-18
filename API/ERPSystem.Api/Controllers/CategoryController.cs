using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Category;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoryController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryController(IConfiguration configuration, ICategoryService categoryService, IMapper mapper)
    {
        _configuration = configuration;
        _categoryService = categoryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get init of page
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiCategoriesInit)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Category)]
    public IActionResult GetInit()
    {
        return Ok(_categoryService.GetInit());
    }
    
    /// <summary>
    /// Get listing category
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiCategories)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Category)]
    public IActionResult Gets(string name, List<int>types, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var filter = new CategoryFilterModel()
        {
            Name = name,
            Types = types,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.CategoryColumn(),
            SortDirection = sortDirection,
        };
        var data = _categoryService.Gets(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<CategoryListModel>()
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
    /// Add new category
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiCategories)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Category)]
    public IActionResult Add([FromBody] CategoryAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        bool result = _categoryService.Add(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete multi category
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiCategories)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Category)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }
        
        bool result = _categoryService.Delete(ids);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Get detail category by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiCategoriesId)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Category)]
    public IActionResult GetById(int id)
    {
        var item = _categoryService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        return Ok(_mapper.Map<CategoryModel>(item));
    }

    /// <summary>
    /// Edit category by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiCategoriesId)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.Category)]
    public IActionResult Edit(int id, [FromBody] CategoryEditModel model)
    {
        var item = _categoryService.GetById(id);
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

        bool result = _categoryService.Edit(model);
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Delete category by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiCategoriesId)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Category)]
    public IActionResult Delete(int id)
    {
        var item = _categoryService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }

        bool result = _categoryService.Delete(new List<int>() { id });
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
}