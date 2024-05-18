using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IUserFolderRepository : IGenericRepository<UserFolder>
{
    UserFolder? GetByUserAndFolder(int userId, int folderId);
    IQueryable<UserFolder> GetByFolder(int folderId);
    IQueryable<UserFolder> GetByUser(int userId);
}

public class UserFolderRepository : GenericRepository<UserFolder>, IUserFolderRepository
{
    private readonly AppDbContext _dbContext;
    
    public UserFolderRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public UserFolder? GetByUserAndFolder(int userId, int folderId)
    {
        return _dbContext.UserFolder.FirstOrDefault(m => m.UserId == userId && m.FolderId == folderId);
    }

    public IQueryable<UserFolder> GetByFolder(int folderId)
    {
        return _dbContext.UserFolder.Where(m => m.FolderId == folderId);
    }

    public IQueryable<UserFolder> GetByUser(int userId)
    {
        return _dbContext.UserFolder.Where(m => m.UserId == userId);
    }
}