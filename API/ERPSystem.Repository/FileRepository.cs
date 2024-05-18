using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IFileRepository : IGenericRepository<DataAccess.Models.File>
{
    DataAccess.Models.File? GetByName(string name, int? folderId);
    IQueryable<DataAccess.Models.File> GetChildFileByFolderId(int? folderId);
    IQueryable<DataAccess.Models.File> GetByIds(List<int> ids);
}

public class FileRepository : GenericRepository<DataAccess.Models.File>, IFileRepository
{
    private readonly AppDbContext _dbContext;
    public FileRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public DataAccess.Models.File? GetByName(string name, int? folderId)
    {
        return _dbContext.File.FirstOrDefault(m => m.FolderId == folderId && m.Name == name);
    }

    public IQueryable<DataAccess.Models.File> GetChildFileByFolderId(int? folderId)
    {
        return _dbContext.File.Where(m => m.FolderId == folderId);
    }

    public IQueryable<DataAccess.Models.File> GetByIds(List<int> ids)
    {
        return _dbContext.File.Where(m => ids.Contains(m.Id));
    }
}