using System.IO.Compression;
using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Driver;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using File = ERPSystem.DataAccess.Models.File;

namespace ERPSystem.Service.Services;

public interface IDriverService
{
    Dictionary<string, object> GetInit();
    Folder? GetFolderById(int id);
    bool AddFolder(FolderAddModel model);
    bool EditFolder(FolderEditModel model);
    bool DeleteMultiFolder(List<int> ids);
    bool IsExistedNameInLevel(string name, int? parentId, int ignoreId);
    bool CanBeEditFolder(int? folderId);
    bool CanBeDeleteFolder(int folderId);
    bool CanBeViewFolder(int folderId);
    bool CanBeEditFile(int fileId);
    bool CanBeDeleteFile(int fileId);
    bool CanBeViewFile(int fileId);
    File? GetFileById(int id);
    File? GetFileByName(string name, int? folderId);
    bool AddFile(FileAddModel model);
    bool EditFile(FileEditModel model);
    bool DeleteMultiFile(List<int> ids);
    bool IsExistedFileNameInLevel(string name, int? folderId, int ignoreId);
    byte[]? GetDataByFileId(int fileId);
    byte[]? GetDataZipByFolderId(int folderId);
    bool ShareFolder(int folderId, DriverShareModel model);
    bool ShareFile(int fileId, DriverShareModel model);
    List<DocumentListModel> GetDocumentByFolderId(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<DocumentListModel> GetAllDocumentShareWithMe(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<UserShareModel> GetListUserSharedOfFolder(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<UserShareModel> GetListUserNotSharedOfFolder(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<UserShareModel> GetListUserSharedOfFile(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<UserShareModel> GetListUserNotSharedOfFile(DriverFilterModel filter, out int totalRecords, out int recordsFiltered);
    bool RemoveShareFolderUser(int folderId, List<int> userIds);
    bool RemoveShareFileUser(int fileId, List<int> userIds);
}

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly HttpContext _httpContext;
    private readonly ILogger _logger;

    public DriverService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
        _httpContext = httpContextAccessor.HttpContext ?? new DefaultHttpContext();
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<DriverService>();
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        // permission type
        var permissions = EnumHelper.ToEnumList<MediaPermission>();
        data.Add("permissions", permissions);
        
        // root folder id
        int myUserId = _httpContext.User.GetUserId();
        int rootFolderId = 0;
        if (myUserId != 0)
        {
            var rootFolder = _unitOfWork.FolderRepository.GetRootFolderByUser(myUserId);
            if (rootFolder != null)
            {
                rootFolderId = rootFolder.Id;
            }
        }
        data.Add("rootFolderId", rootFolderId);
        
        return data;
    }
    
    public Folder? GetFolderById(int id)
    {
        return _unitOfWork.FolderRepository.GetById(id);
    }
    
    public bool AddFolder(FolderAddModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // save in database
                    var folder = _mapper.Map<Folder>(model);
                    _unitOfWork.FolderRepository.Add(folder);
                    _unitOfWork.Save();
                    
                    // assign permission
                    int myUserId = _httpContext.User.GetUserId();
                    if (myUserId == 0)
                        throw new Exception($"User not existed in system userId = 0");
                    _unitOfWork.UserFolderRepository.Add(new UserFolder()
                    {
                        UserId = myUserId,
                        FolderId = folder.Id,
                        PermissionType = (int)MediaPermission.Owner,
                    });
                    _unitOfWork.Save();
                    
                    // permission for other user
                    if (folder.ParentId.HasValue)
                    {
                        var userFolders = _unitOfWork.UserFolderRepository.GetByFolder(folder.ParentId.Value)
                            .Where(m => m.UserId != myUserId);
                        foreach (var item in userFolders)
                        {
                            UserFolder userFolderItem = new UserFolder()
                            {
                                FolderId = folder.Id,
                                UserId = item.UserId,
                                PermissionType = ClonePermissionType(item.PermissionType),
                            };
                            _unitOfWork.UserFolderRepository.Add(userFolderItem);
                        }
                        _unitOfWork.Save();
                    }
                    
                    // create in volume server
                    var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(folder.Id);
                    if (folderFullParent != null)
                    {
                        bool resultCreateFolder = FileHelpers.CreateFolder(GetFullPathByFolder(folderFullParent));
                        if (!resultCreateFolder)
                            throw new Exception("Exception create folder by volume server");
                    }
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });

        return result;
    }
    
    public bool EditFolder(FolderEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var folder = _unitOfWork.FolderRepository.GetById(model.Id);
                    if (folder == null)
                        throw new Exception($"Can not find folder by id = {model.Id}");
                    
                    // edit in volume server
                    var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(folder.Id);
                    if (folderFullParent != null)
                    {
                        bool resultEditNameOfFolder = FileHelpers.EditNameOfFolder(GetFullPathByFolder(folderFullParent), model.Name);
                        if (!resultEditNameOfFolder)
                            throw new Exception("Exception rename folder by volume server");
                    }
                    
                    // edit in database
                    _mapper.Map(model, folder);
                    _unitOfWork.FolderRepository.Update(folder);
                    _unitOfWork.Save();
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }

            }
        });

        return result;
    }

    public bool DeleteMultiFolder(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var folderId in ids)
                    {
                        var folder = _unitOfWork.FolderRepository.GetById(folderId);
                        if (folder == null)
                        {
                            _logger.LogWarning($"Can not find folder by id = {folderId}");
                            continue;
                        }
                        
                        // delete in database
                        _unitOfWork.FolderRepository.Delete(folder);
                        
                        // delete in volume server
                        var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(folder.Id);
                        if (folderFullParent != null)
                        {
                            bool resultDeleteFolder = FileHelpers.DeleteFolder(GetFullPathByFolder(folderFullParent));
                            if (!resultDeleteFolder)
                                throw new Exception("Exception delete folder by volume server");
                        }
                        
                        _unitOfWork.Save();
                    }
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });

        return result;
    }

    public bool IsExistedNameInLevel(string name, int? parentId, int ignoreId)
    {
        var folder = _unitOfWork.FolderRepository.GetByName(name).FirstOrDefault(m => m.ParentId == parentId);
        return folder != null && folder.Id != ignoreId;
    }

    public bool CanBeEditFolder(int? folderId)
    {
        if (!folderId.HasValue)
            return true;

        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
        };
        
        var userFolder = _unitOfWork.UserFolderRepository.GetByUserAndFolder(myUserId, folderId.Value);
        return userFolder != null && permissions.Contains(userFolder.PermissionType);
    }

    public bool CanBeDeleteFolder(int folderId)
    {
        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
        };
        
        var userFolder = _unitOfWork.UserFolderRepository.GetByUserAndFolder(myUserId, folderId);
        return userFolder != null && permissions.Contains(userFolder.PermissionType);
    }

    public bool CanBeViewFolder(int folderId)
    {
        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
            (int)MediaPermission.Viewer,
        };
        
        var userFolder = _unitOfWork.UserFolderRepository.GetByUserAndFolder(myUserId, folderId);
        return userFolder != null && permissions.Contains(userFolder.PermissionType);
    }

    public bool CanBeEditFile(int fileId)
    {
        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
        };

        var userFile = _unitOfWork.UserFileRepository.GetByUserAndFile(myUserId, fileId);
        return userFile != null && permissions.Contains(userFile.PermissionType);
    }
    
    public bool CanBeDeleteFile(int fileId)
    {
        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
        };

        var userFile = _unitOfWork.UserFileRepository.GetByUserAndFile(myUserId, fileId);
        return userFile != null && permissions.Contains(userFile.PermissionType);
    }

    public bool CanBeViewFile(int fileId)
    {
        int myUserId = _httpContext.User.GetUserId();
        var permissions = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
            (int)MediaPermission.Viewer,
        };
        
        var userFile = _unitOfWork.UserFileRepository.GetByUserAndFile(myUserId, fileId);
        return userFile != null && permissions.Contains(userFile.PermissionType);
    }

    public File? GetFileById(int id)
    {
        return _unitOfWork.FileRepository.GetById(id);
    }

    public File? GetFileByName(string name, int? folderId)
    {
        return _unitOfWork.FileRepository.GetByName(name, folderId);
    }
    
    public bool AddFile(FileAddModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // add file in database
                    File file = new File()
                    {
                        Name = model.File.FileName,
                        FolderId = model.FolderId,
                        Size = model.File.Length,
                    };
                    _unitOfWork.FileRepository.Add(file);
                    _unitOfWork.Save();
                    
                    // update permission
                    int myUserId = _httpContext.User.GetUserId();
                    if (myUserId == 0)
                        throw new Exception($"User not existed in system userId = 0");
                    UserFile userFile = new UserFile()
                    {
                        FileId = file.Id,
                        UserId = myUserId,
                        PermissionType = (int)MediaPermission.Owner,
                    };
                    _unitOfWork.UserFileRepository.Add(userFile);
                    _unitOfWork.Save();
                    
                    // permission for other user
                    if (file.FolderId.HasValue)
                    {
                        var userFolders = _unitOfWork.UserFolderRepository.GetByFolder(file.FolderId.Value)
                            .Where(m => m.UserId != myUserId);
                        foreach (var item in userFolders)
                        {
                            UserFile userFileItem = new UserFile()
                            {
                                FileId = file.Id,
                                UserId = item.UserId,
                                PermissionType = ClonePermissionType(item.PermissionType),
                            };
                            _unitOfWork.UserFileRepository.Add(userFileItem);
                        }
                        _unitOfWork.Save();
                    }
                    
                    // add file in volume server
                    var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(model.FolderId ?? 0);
                    string pathFile = $"{GetFullPathByFolder(folderFullParent)}/{model.File.FileName}";
                    bool resultAddFile = FileHelpers.SaveFileByIFormFile(model.File, pathFile);
                    if (!resultAddFile)
                        throw new Exception("Exception add file by volume server");
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });

        return result;
    }

    public bool EditFile(FileEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var file = _unitOfWork.FileRepository.GetById(model.Id);
                    if (file == null)
                        throw new Exception($"Can not find file by id = {model.Id}");
                    
                    // edit in volume server
                    var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(file.FolderId ?? 0);
                    string fullPath = GetFullPathByFolder(folderFullParent);
                    string oldPathFile = $"{fullPath}/{file.Name}";
                    string newPathFile = $"{fullPath}/{model.Name}";
                    bool resultEditFile = FileHelpers.EditNameOfFile(oldPathFile, newPathFile);
                    if (!resultEditFile)
                        throw new Exception("Exception IO edit name of file");
                    
                    // edit in database
                    _mapper.Map(model, file);
                    _unitOfWork.FileRepository.Update(file);
                    _unitOfWork.Save();
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }

            }
        });

        return result;
    }
    
    public bool DeleteMultiFile(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var id in ids)
                    {
                        var file = _unitOfWork.FileRepository.GetById(id);
                        if (file == null)
                            throw new Exception($"Can not find file by id = {id}");

                        // delete file in volume server
                        var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(file.FolderId ?? 0);
                        string pathFile = $"{GetFullPathByFolder(folderFullParent)}/{file.Name}";
                        bool resultDeleteFile = FileHelpers.DeleteFileFromLink(pathFile);
                        if (!resultDeleteFile)
                            throw new Exception($"Exception delete file IO by id ={file.Id}");

                        // delete file in database
                        _unitOfWork.FileRepository.Delete(file);
                        _unitOfWork.Save();
                    }
                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }

    public bool IsExistedFileNameInLevel(string name, int? folderId, int ignoreId)
    {
        var file = _unitOfWork.FileRepository.GetByName(name, folderId);
        return file != null && file.Id != ignoreId;
    }

    public byte[]? GetDataByFileId(int fileId)
    {
        try
        {
            var file = _unitOfWork.FileRepository.GetById(fileId);
            if (file == null)
                throw new Exception($"Can not find file by id = {fileId}");
            
            var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(file.FolderId ?? 0);
            string pathFile = $"{GetFullPathByFolder(folderFullParent)}/{file.Name}";

            return System.IO.File.ReadAllBytes(pathFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            return null;
        }
    }

    public byte[]? GetDataZipByFolderId(int folderId)
    {
        try
        {
            var folder = _unitOfWork.FolderRepository.GetById(folderId);
            if (folder == null)
                throw new Exception($"Can not find folder by id = {folderId}");
            
            var folderFullParent = _unitOfWork.FolderRepository.GetFullParentFolder(folder.Id);
            string fullPath = GetFullPathByFolder(folderFullParent);
            string zipFilePath = $"{Constants.MediaConfig.BaseFolderData}/{Guid.NewGuid().ToString()}.zip";
            ZipFile.CreateFromDirectory(fullPath, zipFilePath);
            var data = System.IO.File.ReadAllBytes($"{zipFilePath}");
            FileHelpers.DeleteFileFromLink(zipFilePath);
            
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            return null;
        }
    }

    public bool ShareFolder(int folderId, DriverShareModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var folder = _unitOfWork.FolderRepository.GetById(folderId);
                    if (folder == null)
                        throw new Exception($"Can not get folder by id ={folderId}");
                    
                    ShareFolderNotTracking(folderId, model);
                    ShareChildInFolder(folderId, model);
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }
    
    public bool ShareFile(int fileId, DriverShareModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var file = _unitOfWork.FileRepository.GetById(fileId);
                    if (file == null)
                        throw new Exception($"Can not get file by id ={fileId}");
                    
                    ShareFileNotTracking(fileId, model);
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }

    public List<DocumentListModel> GetDocumentByFolderId(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        List<DocumentListModel> data = new List<DocumentListModel>();
        List<int> permissionEdit = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
        };
        int myUserId = _httpContext.User.GetUserId();
        
        // get folder
        var folders = _unitOfWork.FolderRepository.GetChildFolderByParentId(filter.FolderId)
            .Include(m => m.UserFolder).ThenInclude(m => m.User)
            .AsEnumerable<Folder>()
            .Select(m => new DocumentListModel()
            {
                Id = $"{Constants.MediaConfig.FolderAliasId}{Constants.MediaConfig.SplitText}{m.Id}",
                Name = m.Name,
                Description = m.Description,
                FolderId = m.Id,
                UpdatedOn = m.UpdatedOn.ConvertDefaultDateTimeToString(),
                CreatedOn = m.CreatedOn.ConvertDefaultDateTimeToString(),
                OwnerId = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.UserId ?? 0,
                OwnerName = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Name ?? string.Empty,
                OwnerAvatar = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Avatar ?? string.Empty,
                Shared = m.UserFolder.Count > 1,
                CanBeEdit = m.UserFolder.Any(n => n.UserId == myUserId && permissionEdit.Contains(n.PermissionType)),
            });
        data.AddRange(folders);

        // get file
        var files = _unitOfWork.FileRepository.GetChildFileByFolderId(filter.FolderId)
            .Include(m => m.UserFile).ThenInclude(m => m.User)
            .AsEnumerable<File>()
            .Select(m => new DocumentListModel()
            {
                Id = $"{Constants.MediaConfig.FileAliasId}{Constants.MediaConfig.SplitText}{m.Id}",
                Name = m.Name,
                Description = m.Description,
                FileId = m.Id,
                UpdatedOn = m.UpdatedOn.ConvertDefaultDateTimeToString(),
                CreatedOn = m.CreatedOn.ConvertDefaultDateTimeToString(),
                OwnerId = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.UserId ?? 0,
                OwnerName = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Name ?? string.Empty,
                OwnerAvatar = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Avatar ?? string.Empty,
                Size = m.Size,
                Shared = m.UserFile.Count > 1,
                CanBeEdit = m.UserFile.Any(n => n.UserId == myUserId && permissionEdit.Contains(n.PermissionType)),
            });
        data.AddRange(files);

        totalRecords = data.Count;
        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower())).ToList();
        }
        recordsFiltered = data.Count;
        data = data.AsQueryable().OrderBy($"{filter.SortColumn} {filter.SortDirection}").ToList();
        if (filter.PageSize > 0)
            data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        
        return data;
    }

    public List<DocumentListModel> GetAllDocumentShareWithMe(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        // init data
        List<DocumentListModel> data = new List<DocumentListModel>();
        Folder? rootFolder;
        int rootFolderId = 0;
        List<int> permissionEdit = new List<int>()
        {
            (int)MediaPermission.Owner,
            (int)MediaPermission.Editor,
        };
        int myUserId = _httpContext.User.GetUserId();

        // folder
        rootFolder = _unitOfWork.FolderRepository.GetRootFolderByUser(myUserId);
        rootFolderId = rootFolder?.Id ?? 0;
        List<int> ignoreFolderIds = new List<int>() { rootFolderId };
        var ignoreFolders = GetFullTreeFolderByParentId(rootFolderId, ref ignoreFolderIds);
        var folderIds = _unitOfWork.UserFolderRepository.GetByUser(myUserId)
            .Where(m => !ignoreFolderIds.Contains(m.FolderId))
            .Select(m => m.FolderId).ToList();
        var folders = _unitOfWork.FolderRepository.GetByIds(folderIds)
            .Include(m => m.UserFolder).ThenInclude(m => m.User)
            .ToList();
        List<Folder> treeFolders = new List<Folder>();
        foreach (var folder in folders)
        {
            folder.ChildFolder = GenerateTree(folders, folder);
            treeFolders.Add(folder);
            folder.Parent = folders.FirstOrDefault(m => m.Id == folder.ParentId);
        }

        data.AddRange(treeFolders.Where(m => m.Parent == null).Select(m => new DocumentListModel()
        {
            Id = $"{Constants.MediaConfig.FolderAliasId}{Constants.MediaConfig.SplitText}{m.Id}",
            Name = m.Name,
            Description = m.Description,
            FolderId = m.Id,
            UpdatedOn = m.UpdatedOn.ConvertDefaultDateTimeToString(),
            CreatedOn = m.CreatedOn.ConvertDefaultDateTimeToString(),
            OwnerId = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.UserId ?? 0,
            OwnerName = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Name ?? string.Empty,
            OwnerAvatar = m.UserFolder.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Avatar ?? string.Empty,
            Shared = m.UserFolder.Count > 1,
            CanBeEdit = m.UserFolder.Any(n => n.UserId == myUserId && permissionEdit.Contains(n.PermissionType)),
        }));
        
        // file
        var fileIds = _unitOfWork.UserFileRepository.GetByUser(myUserId).Select(m => m.FileId).ToList();
        var files = _unitOfWork.FileRepository.GetByIds(fileIds)
            .Where(m => !m.FolderId.HasValue || (!folderIds.Contains(m.FolderId.Value) && !ignoreFolderIds.Contains(m.FolderId.Value)))
            .Include(m => m.UserFile).ThenInclude(m => m.User)
            .ToList();
        data.AddRange(files.Select(m => new DocumentListModel()
        {
            Id = $"{Constants.MediaConfig.FileAliasId}{Constants.MediaConfig.SplitText}{m.Id}",
            Name = m.Name,
            Description = m.Description,
            FileId = m.Id,
            UpdatedOn = m.UpdatedOn.ConvertDefaultDateTimeToString(),
            CreatedOn = m.CreatedOn.ConvertDefaultDateTimeToString(),
            OwnerId = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.UserId ?? 0,
            OwnerName = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Name ?? string.Empty,
            OwnerAvatar = m.UserFile.FirstOrDefault(n => n.PermissionType == (int)MediaPermission.Owner)?.User?.Avatar ?? string.Empty,
            Size = m.Size,
            Shared = m.UserFile.Count > 1,
            CanBeEdit = m.UserFile.Any(n => n.UserId == myUserId && permissionEdit.Contains(n.PermissionType)),
        }));

        totalRecords = data.Count;
        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name)).ToList();
        }
        recordsFiltered = data.Count;
        data = data.AsQueryable().OrderBy($"{filter.SortColumn} {filter.SortDirection}").ToList();
        if (filter.PageSize > 0)
            data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        
        return data;
    }

    public List<UserShareModel> GetListUserSharedOfFolder(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var dataOwner = _unitOfWork.UserFolderRepository.Gets()
            .Include(m => m.User).ThenInclude(m => m.Department)
            .Where(m => m.FolderId == filter.FolderId && m.PermissionType == (int)MediaPermission.Owner)
            .AsEnumerable();
        
        var data = _unitOfWork.UserFolderRepository.Gets()
            .Include(m => m.User).ThenInclude(m => m.Department)
            .Where(m => m.FolderId == filter.FolderId && m.PermissionType != (int)MediaPermission.Owner);

        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.User.Name.ToLower().Contains(filter.Name.ToLower()) || m.User.Email.ToLower().Contains(filter.Name.ToLower()));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var result = new List<UserShareModel>();
        result.AddRange(dataOwner.Select(_mapper.Map<UserShareModel>));
        result.AddRange(data.AsEnumerable().Select(_mapper.Map<UserShareModel>));
        return result;
    }

    public List<UserShareModel> GetListUserNotSharedOfFolder(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var userSharedIds = _unitOfWork.UserFolderRepository.GetByFolder(filter.FolderId).Select(m => m.UserId)
            .AsEnumerable();
        var data = _unitOfWork.UserRepository.Gets()
            .Include(m => m.Department)
            .Where(m => !userSharedIds.Contains(m.Id));

        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()) || m.Email.ToLower().Contains(filter.Name.ToLower()));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        
        return data.AsEnumerable().Select(_mapper.Map<UserShareModel>).ToList();
    }
    
    public List<UserShareModel> GetListUserSharedOfFile(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var dataOwner = _unitOfWork.UserFileRepository.Gets()
            .Include(m => m.User).ThenInclude(m => m.Department)
            .Where(m => m.FileId == filter.FileId && m.PermissionType == (int)MediaPermission.Owner)
            .AsEnumerable();
        
        var data = _unitOfWork.UserFileRepository.Gets()
            .Include(m => m.User).ThenInclude(m => m.Department)
            .Where(m => m.FileId == filter.FileId && m.PermissionType != (int)MediaPermission.Owner);

        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.User.Name.ToLower().Contains(filter.Name.ToLower()) || m.User.Email.ToLower().Contains(filter.Name.ToLower()));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var result = new List<UserShareModel>();
        result.AddRange(dataOwner.Select(_mapper.Map<UserShareModel>));
        result.AddRange(data.AsEnumerable().Select(_mapper.Map<UserShareModel>));
        return result;
    }
    
    public List<UserShareModel> GetListUserNotSharedOfFile(DriverFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var userSharedIds = _unitOfWork.UserFileRepository.Gets(m => m.FileId == filter.FileId)
            .Select(m => m.UserId).AsEnumerable();
        var data = _unitOfWork.UserRepository.Gets()
            .Include(m => m.Department)
            .Where(m => !userSharedIds.Contains(m.Id));

        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()) || m.Email.ToLower().Contains(filter.Name.ToLower()));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        
        return data.AsEnumerable().Select(_mapper.Map<UserShareModel>).ToList();
    }

    public bool RemoveShareFolderUser(int folderId, List<int> userIds)
    {
        try
        {
            _unitOfWork.UserFolderRepository.Delete(m =>
                m.FolderId == folderId
                && userIds.Contains(m.UserId)
                && m.PermissionType != (int)MediaPermission.Owner);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            return false;
        }
    }
    
    public bool RemoveShareFileUser(int fileId, List<int> userIds)
    {
        try
        {
            _unitOfWork.UserFileRepository.Delete(m =>
                m.FileId == fileId
                && userIds.Contains(m.UserId)
                && m.PermissionType != (int)MediaPermission.Owner);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            return false;
        }
    }
    
    private void ShareChildInFolder(int folderId, DriverShareModel model)
    {
        // share folder
        var folderIds = _unitOfWork.FolderRepository.GetChildFolderByParentId(folderId).Select(m => m.Id).ToList();
        foreach (var folderIdItem in folderIds)
        {
            ShareFolderNotTracking(folderIdItem, model);
            ShareChildInFolder(folderIdItem, model);
        }
        
        // share file
        var fileIds = _unitOfWork.FileRepository.GetChildFileByFolderId(folderId).Select(m => m.Id).ToList();
        foreach (int fileIdItem in fileIds)
        {
            ShareFileNotTracking(fileIdItem, model);
        }
    }
    
    private void ShareFolderNotTracking(int folderId, DriverShareModel model)
    {
        foreach (int userId in model.UserIds)
        {
            var userFolder = _unitOfWork.UserFolderRepository.GetByUserAndFolder(userId, folderId);
            if (userFolder == null)
            {
                userFolder = new UserFolder()
                {
                    FolderId = folderId,
                    UserId = userId,
                    PermissionType = model.PermissionId,
                };
                _unitOfWork.UserFolderRepository.Add(userFolder);
            }
            else
            {
                if (userFolder.PermissionType == (int)MediaPermission.Owner)
                {
                    Console.WriteLine($"[Warning share folder][permission {model.PermissionId}]: Can not share folder(id={folderId}) - user(id={userId}) permission owner");
                }
                else
                {
                    userFolder.PermissionType = model.PermissionId;
                    _unitOfWork.UserFolderRepository.Update(userFolder);
                }
            }
            
            _unitOfWork.Save();
        }
    }
    
    private void ShareFileNotTracking(int fileId, DriverShareModel model)
    {
        foreach (var userId in model.UserIds)
        {
            var userFile = _unitOfWork.UserFileRepository.GetByUserAndFile(userId, fileId);
            if (userFile == null)
            {
                userFile = new UserFile()
                {
                    FileId = fileId,
                    UserId = userId,
                    PermissionType = model.PermissionId,
                };
                _unitOfWork.UserFileRepository.Add(userFile);
            }
            else
            {
                if (userFile.PermissionType == (int)MediaPermission.Owner)
                {
                    Console.WriteLine($"[Warning share file][permission {model.PermissionId}]: Can not share file(id={fileId}) - user(id={userId}) permission owner");
                }
                else
                {
                    userFile.PermissionType = model.PermissionId;
                    _unitOfWork.UserFileRepository.Update(userFile);
                }
            }
        }
        
        _unitOfWork.Save();
    }
    
    private string GetFullPathByFolder(Folder? folder)
    {
        List<string> folderNames = new List<string>();
        if (folder != null)
        {
            folderNames.Add(folder.Name);
            var parent = folder.Parent;
            while (parent != null)
            {
                folderNames.Add(parent.Name);
                parent = parent.Parent;
            }    
        }
        
        folderNames.Add(Constants.MediaConfig.BaseFolderData);
        string fullPath = "";
        for (int i = folderNames.Count - 1; i >= 0; i--)
        {
            fullPath += $"{folderNames[i]}/";
        }

        return fullPath.Substring(0, fullPath.Length - 1);
    }

    private int ClonePermissionType(int permission)
    {
        switch (permission)
        {
            case (int)MediaPermission.Owner:
            case (int)MediaPermission.Editor:
                return (int)MediaPermission.Editor;
            case (int)MediaPermission.Viewer:
                return (int)MediaPermission.Viewer;
            default:
                return (int)MediaPermission.Viewer;
        }
    }

    private List<Folder> GetFullTreeFolderByParentId(int parentId, ref List<int> listFolderIds)
    {
        List<Folder> data = new List<Folder>();
        var folders = _unitOfWork.FolderRepository.GetChildFolderByParentId(parentId).ToList();
        if (folders.Count > 0)
        {
            listFolderIds.AddRange(folders.Select(m => m.Id));
            data.AddRange(folders);
            foreach (var item in data)
            {
                item.ChildFolder = GetFullTreeFolderByParentId(item.Id, ref listFolderIds);
            }
        }
        
        return data;
    }
    
    private List<Folder> GenerateTree(List<Folder> data, Folder rootItem)
    {
        List<Folder> result = new List<Folder>();
        var children = data.Where(m => m.ParentId == rootItem.Id);
        foreach (var item in children)
        {
            item.ChildFolder = GenerateTree(data, item);
            result.Add(item);
        }

        return result;
    }
}