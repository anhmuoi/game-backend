using AutoMapper;
using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.ItemNft;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ItemNftController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IItemNftService _itemNftService;
    private readonly IMapper _mapper;

    public ItemNftController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IItemNftService itemNftService, IMapper mapper)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _itemNftService = itemNftService;
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
    [Route(Constants.Route.ApiItemNfts)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult Gets(string search, int pageNumber = 1, int pageSize = 10,
        string sortColumn = "Name",
        string sortDirection = "asc")
    {
        var itemNfts = _itemNftService.GetPaginated(search, pageNumber, pageSize, sortColumn.ItemNftColumn(),
            sortDirection, out var recordsTotal,
            out var recordsFiltered);

        var pagingData = new PagingData<ItemNftListModel>
        {
            Data = itemNfts,
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
    [Route(Constants.Route.ApiItemNfts)]
    [CheckPermission(PagePermission.ActionName.Add + PagePermission.Page.Meeting)]
    public IActionResult Add([FromBody] ItemNftAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }
        // save image coupon
        if (model.Image.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            string path = $"{Constants.ImageConfig.BaseFolderItemNft}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Image, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Image = $"{hostApi}/static/{path}";
        }

        int result = _itemNftService.AddItemNft(model);
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
    [Route(Constants.Route.ApiItemNfts)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult DeleteMulti(List<int> ids)
    {
        if (ids is not { Count: > 0 })
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }


        bool result = _itemNftService.DeleteMultiItemNfts(ids);
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
    [Route(Constants.Route.ApiItemNftsId)]
    [CheckPermission(PagePermission.ActionName.View + PagePermission.Page.Meeting)]
    public IActionResult GetById(int id)
    {
        var item = _itemNftService.GetById(id);
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
    [Route(Constants.Route.ApiItemNftsId)]
    [CheckPermission(PagePermission.ActionName.Edit + PagePermission.Page.Meeting)]
    public IActionResult Edit(int id, [FromBody] ItemNftEditModel model)
    {
        var item = _itemNftService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        // save image coupon
        if (model.Image.IsTextBase64())
        {
            string hostApi = _configuration.GetSection(Constants.Settings.DefineHostApiConfig).Value;
            string path = $"{Constants.ImageConfig.BaseFolderItemNft}/{Guid.NewGuid().ToString()}.jpg";
            bool isSaveImage = FileHelpers.SaveFileImage(model.Image, path);
            if (!isSaveImage)
                return new ApiErrorResult(StatusCodes.Status500InternalServerError, MessageResource.SystemError);

            model.Image = $"{hostApi}/static/{path}";
        }

        model.Id = id;
        ModelState.Clear();
        if (!TryValidateModel(model))
        {
            return new ValidationFailedResult(ModelState);
        }

   

        bool result = _itemNftService.UpdateItemNft(model);
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
    [Route(Constants.Route.ApiItemNftsId)]
    [CheckPermission(PagePermission.ActionName.Delete + PagePermission.Page.Meeting)]
    public IActionResult Delete(int id)
    {
        var item = _itemNftService.GetById(id);
        if (item == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
     
        bool result = _itemNftService.DeleteMultiItemNfts(new List<int>{id});
        if (!result)
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        }

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }
}