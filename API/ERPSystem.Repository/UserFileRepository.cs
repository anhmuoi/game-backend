using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IUserFileRepository : IGenericRepository<UserFile>
{
    UserFile? GetByUserAndFile(int userId, int fileId);
    IQueryable<UserFile> GetByUser(int userId);
}

public class UserFileRepository : GenericRepository<UserFile>, IUserFileRepository
{
    private readonly AppDbContext _dbContext;
    public UserFileRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public UserFile? GetByUserAndFile(int userId, int fileId)
    {
        return _dbContext.UserFile.FirstOrDefault(m => m.UserId == userId && m.FileId == fileId);
    }

    public IQueryable<UserFile> GetByUser(int userId)
    {
        return _dbContext.UserFile.Where(m => m.UserId == userId);
    }
}