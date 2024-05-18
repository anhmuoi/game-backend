using ERPSystem.Api.Infrastructure.Mapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataModel;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Driver;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    /// <summary>
    /// Get init page
    /// </summary>
    /// <param name="rootFolderId">
    /// null: my-drive, &lt;br&gt;
    /// 0: shared-with-me, &lt;br&gt;
    /// other: folders &lt;br&gt;
    /// </param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDriverInit)]
    public IActionResult GetInit(int? rootFolderId)
    {
        var data = _driverService.GetInit();
        string rootFolderName = DriverResource.lblMyDrive;
        if (rootFolderId.HasValue)
        {
            if (rootFolderId == 0)
            {
                rootFolderName = DriverResource.lblSharedWithMe;
            }
            else
            {
                var rootFolder = _driverService.GetFolderById(rootFolderId.Value);
                if (rootFolder == null)
                    return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
                if (!_driverService.CanBeViewFolder(rootFolder.Id))
                    return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({rootFolder.Name})");

                rootFolderName = rootFolder.Name;
            }
            data.Remove("rootFolderId");
            data.Add("rootFolderId", rootFolderId.Value);
        }
        
        data.Add("rootFolderName", rootFolderName);
        return Ok(data);
    }
    
    /// <summary>
    /// Add new folder
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFolders)]
    public IActionResult AddFolder([FromBody] FolderAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }
        
        bool result = _driverService.AddFolder(model);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);
        
        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }
    
    /// <summary>
    /// Delete folder by ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiFolders)]
    public IActionResult DeleteMultiFolder(List<int> ids)
    {
        if (ids is not { Count: > 0 })
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);

        foreach (int id in ids)
        {
            if (!_driverService.CanBeDeleteFolder(id))
            {
                var folder = _driverService.GetFolderById(id);
                string folderName = folder != null ? $"({folder.Name})" : "";
                return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFolder + folderName);
            }
        }

        bool result = _driverService.DeleteMultiFolder(ids);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Edit folder by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiFoldersId)]
    public IActionResult EditFolder(int id, [FromBody] FolderEditModel model)
    {
        var folder = _driverService.GetFolderById(id);
        if (folder == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        model.Id = id;
        if (!_driverService.CanBeEditFolder(model.Id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionInFolder);

        if (_driverService.IsExistedNameInLevel(model.Name, folder.ParentId, model.Id))
            return new ApiErrorResult(StatusCodes.Status400BadRequest, DriverResource.msgFolderNameExisted);

        bool result = _driverService.EditFolder(model);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Delete folder by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiFoldersId)]
    public IActionResult DeleteFolder(int id)
    {
        if (!_driverService.CanBeDeleteFolder(id))
        {
            var folder = _driverService.GetFolderById(id);
            string folderName = folder != null ? $"({folder.Name})" : "";
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFolder + folderName);
        }
        
        bool result = _driverService.DeleteMultiFolder(new List<int>() { id });
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Download folder by id (this folder will compress zip file)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFoldersIdDownload)]
    public IActionResult DownloadFolder(int id)
    {
        var folder = _driverService.GetFolderById(id);
        if (folder == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        if (!_driverService.CanBeViewFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({folder.Name})");

        var data = _driverService.GetDataZipByFolderId(id);
        if (data == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        return File(data, "application/zip", $"{folder.Name}.zip");
    }

    /// <summary>
    /// Share folder
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFoldersIdShare)]
    public IActionResult ShareFolder(int id, [FromBody] DriverShareModel model)
    {
        var folder = _driverService.GetFolderById(id);
        if (folder == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        
        if (!_driverService.CanBeEditFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionInFolder);

        if (!ModelState.IsValid)
            return new ValidationFailedResult(ModelState);
        
        bool result = _driverService.ShareFolder(id, model);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Get user shared of folder
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFoldersIdShare)]
    public IActionResult GetUserShared(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var rootFolder = _driverService.GetFolderById(id);
        if (rootFolder == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({rootFolder.Name})");

        var filter = new DriverFilterModel()
        {
            FolderId = id,
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.UserShareModelColumn(),
            SortDirection = sortDirection,
        };
        var data = _driverService.GetListUserSharedOfFolder(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserShareModel>()
        {
            Data = data,
            Meta = { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Get user not shared of folder
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFoldersIdNotShare)]
    public IActionResult GetUserNotShared(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var rootFolder = _driverService.GetFolderById(id);
        if (rootFolder == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({rootFolder.Name})");

        var filter = new DriverFilterModel()
        {
            FolderId = id,
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.UserNotShareModelColumn(),
            SortDirection = sortDirection,
        };
        var data = _driverService.GetListUserNotSharedOfFolder(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserShareModel>()
        {
            Data = data,
            Meta = { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Un share folder of user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFoldersIdNotShare)]
    public IActionResult RemoveShareFolderUser(int id, [FromBody] DriverShareModel model)
    {
        var rootFolder = _driverService.GetFolderById(id);
        if (rootFolder == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({rootFolder.Name})");

        bool result = _driverService.RemoveShareFolderUser(id, model.UserIds);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        
        return new ApiErrorResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Add file in folder
    /// </summary>
    /// <param name="file"></param>
    /// <param name="folderId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFiles)]
    public IActionResult AddFile([FromForm] IFormFile? file, [FromForm] int? folderId)
    {
        if (!_driverService.CanBeEditFolder(folderId))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionInFolder);
        
        if (file == null || file.Length == 0)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        }

        if (_driverService.GetFileByName(file.FileName, folderId) != null)
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, DriverResource.msgFileNameExisted);
        }

        bool result = _driverService.AddFile(new FileAddModel()
        {
            File = file,
            FolderId = folderId,
        });

        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgCreateNewFailed);

        return new ApiSuccessResult(StatusCodes.Status201Created, MessageResource.msgCreateNewSuccess);
    }

    /// <summary>
    /// Delete file by ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiFiles)]
    public IActionResult DeleteMultiFile(List<int> ids)
    {
        if (ids is not { Count: > 0 })
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
        
        foreach (int id in ids)
        {
            if (!_driverService.CanBeDeleteFile(id))
            {
                var file = _driverService.GetFileById(id);
                string fileName = file != null ? $"({file.Name})" : "";
                return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFile + fileName);
            }
        }
        
        bool result = _driverService.DeleteMultiFile(ids);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Edit file by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route(Constants.Route.ApiFilesId)]
    public IActionResult EditFile(int id, [FromBody] FileEditModel model)
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        model.Id = id;
        if (!_driverService.CanBeEditFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFile + $"({file.Name})");

        if (_driverService.IsExistedFileNameInLevel(model.Name, file.FolderId, model.Id))
            return new ApiErrorResult(StatusCodes.Status400BadRequest, DriverResource.msgFileNameExisted);

        bool result = _driverService.EditFile(model);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Delete file by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route(Constants.Route.ApiFilesId)]
    public IActionResult DeleteFile(int id)
    {
        if (!_driverService.CanBeDeleteFile(id))
        {
            var file = _driverService.GetFileById(id);
            string fileName = file != null ? $"({file.Name})" : "";
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFile + fileName);
        }

        bool result = _driverService.DeleteMultiFile(new List<int>() { id });
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgDeleteFailed);
        
        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgDeleteSuccess);
    }

    /// <summary>
    /// Download file by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFilesIdDownload)]
    public IActionResult DownloadFile(int id)
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        if (!_driverService.CanBeViewFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({file.Name})");

        var data = _driverService.GetDataByFileId(id);
        if (data == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        return File(data, FileHelpers.GetContentTypeByFileName(file.Name), file.Name);
    }

    /// <summary>
    /// Share file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFilesIdShare)]
    public IActionResult ShareFile(int id, [FromBody] DriverShareModel model)
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);

        if (!_driverService.CanBeEditFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgNotPermissionDeleteFile + $"({file.Name})");
        
        if (!ModelState.IsValid)
            return new ValidationFailedResult(ModelState);
        
        bool result = _driverService.ShareFile(id, model);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);

        return new ApiSuccessResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }
    
    /// <summary>
    /// Get user shared of file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFilesIdShare)]
    public IActionResult GetUserSharedFile(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({file.Name})");

        var filter = new DriverFilterModel()
        {
            FileId = id,
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.UserShareFileModelColumn(),
            SortDirection = sortDirection,
        };
        var data = _driverService.GetListUserSharedOfFile(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserShareModel>()
        {
            Data = data,
            Meta = { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };
        return Ok(pagingData);
    }
    
    /// <summary>
    /// Get user not shared of file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFilesIdNotShare)]
    public IActionResult GetUserFileNotShared(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({file.Name})");

        var filter = new DriverFilterModel()
        {
            FileId = id,
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.UserNotShareModelColumn(),
            SortDirection = sortDirection,
        };
        var data = _driverService.GetListUserNotSharedOfFile(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<UserShareModel>()
        {
            Data = data,
            Meta = { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };
        return Ok(pagingData);
    }

    /// <summary>
    /// Un share user of file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiFilesIdNotShare)]
    public IActionResult RemoveShareFileUser(int id, [FromBody] DriverShareModel model)
    {
        var file = _driverService.GetFileById(id);
        if (file == null)
            return new ApiErrorResult(StatusCodes.Status400BadRequest, MessageResource.BadRequest);
            
        if (!_driverService.CanBeViewFile(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({file.Name})");

        bool result = _driverService.RemoveShareFileUser(id, model.UserIds);
        if (!result)
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, MessageResource.msgUpdateFailed);
        
        return new ApiErrorResult(StatusCodes.Status200OK, MessageResource.msgUpdateSuccess);
    }

    /// <summary>
    /// Get all document by folder id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiFoldersIdDocument)]
    public IActionResult GetAllDocumentByFolderId(int id, string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var folder = _driverService.GetFolderById(id);
        if (folder == null)
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        
        if (!_driverService.CanBeViewFolder(id))
            return new ApiErrorResult(StatusCodes.Status403Forbidden, DriverResource.msgPermissionDenied + $"({folder.Name})");
        
        var filter = new DriverFilterModel()
        {
            FolderId = id,
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.DocumentListModelColumn(),
            SortDirection = sortDirection,
        };

        var data = _driverService.GetDocumentByFolderId(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<DocumentListModel>()
        {
            Data = data,
            Meta = new Meta() { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };

        return Ok(pagingData);
    }

    /// <summary>
    /// Get all document (share with me)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    [HttpGet]
    [Route(Constants.Route.ApiDriverSharedWithMe)]
    public IActionResult GetAllDocumentShareWithMe(string name, int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", string sortDirection = "asc")
    {
        var filter = new DriverFilterModel()
        {
            Name = name,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn.DocumentListModelColumn(),
            SortDirection = sortDirection,
        };

        var data = _driverService.GetAllDocumentShareWithMe(filter, out int totalRecords, out int recordsFiltered);
        var pagingData = new PagingData<DocumentListModel>()
        {
            Data = data,
            Meta = new Meta() { RecordsTotal = totalRecords, RecordsFiltered = recordsFiltered },
        };

        return Ok(pagingData);
    }
}