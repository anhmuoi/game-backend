using System.Data.Entity;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IFolderRepository : IGenericRepository<Folder>
{
    IQueryable<Folder> GetByName(string name);
    Folder? GetFullParentFolder(int id);
    IQueryable<Folder> GetChildFolderByParentId(int parentId);
    Folder? GetRootFolderByUser(int userId);
    IQueryable<Folder> GetByIds(List<int> ids);
    void CreateRootFolderForUser(int userId);
}

public class FolderRepository : GenericRepository<Folder>, IFolderRepository
{
    private readonly AppDbContext _dbContext;
    public FolderRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public IQueryable<Folder> GetByName(string name)
    {
        return _dbContext.Folder.Where(m => m.Name == name);
    }

    public Folder? GetFullParentFolder(int id)
    {
        var folder = _dbContext.Folder.FirstOrDefault(m => m.Id == id);
        if (folder != null)
        {
            folder.Parent = GetParentFolder(folder.ParentId);
            return folder;   
        }
        else
        {
            return null;
        }
    }

    public IQueryable<Folder> GetChildFolderByParentId(int parentId)
    {
        return _dbContext.Folder.Where(m => m.ParentId == parentId);
    }

    public Folder? GetRootFolderByUser(int userId)
    {
        var userFolder = _dbContext.UserFolder.Include(m => m.Folder).FirstOrDefault(m => m.UserId == userId && !m.Folder.ParentId.HasValue);
        if (userFolder != null)
        {
            return _dbContext.Folder.FirstOrDefault(m => m.Id == userFolder.FolderId);
        }
        return null;
    }

    public IQueryable<Folder> GetByIds(List<int> ids)
    {
        return _dbContext.Folder.Where(m => ids.Contains(m.Id));
    }

    public void CreateRootFolderForUser(int userId)
    {
        try
        {
            bool result = FileHelpers.CreateFolder($"{Constants.MediaConfig.BaseFolderData}/{userId}");
            if (result)
            {
                // add folder
                var folderItem = new Folder()
                {
                    Name = $"{userId}",
                    Description = $"{userId}",
                };
                _dbContext.Folder.Add(folderItem);
                _dbContext.SaveChanges();

                // add permission
                _dbContext.UserFolder.Add(new UserFolder()
                {
                    UserId = userId,
                    FolderId = folderItem.Id,
                    PermissionType = (int)MediaPermission.Owner,
                });
                _dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    private Folder? GetParentFolder(int? parentId)
    {
        if (parentId.HasValue)
        {
            var parent = _dbContext.Folder.FirstOrDefault(m => m.Id == parentId);
            if (parent != null)
            {
                parent.Parent = GetParentFolder(parent.ParentId);
                return parent;
            }
        }

        return null;
    }
}