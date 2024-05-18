using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IFolderLogRepository : IGenericRepository<FolderLog>
{
    IQueryable<FolderLog> GetByIds(List<int> ids);
    IQueryable<FolderLog> GetByIdsAndChild(List<int> ids);
    IQueryable<FolderLog> GetByName(string name);
}

public class FolderLogRepository : GenericRepository<FolderLog>, IFolderLogRepository
{
    private readonly AppDbContext _dbContext;
    public FolderLogRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
    public IQueryable<FolderLog> GetByIds(List<int> ids)
    {
        return _dbContext.FolderLog.Where(x => ids.Contains(x.Id));
    }

    public IQueryable<FolderLog> GetByIdsAndChild(List<int> ids)
    {
        var folderIds = _dbContext.FolderLog
            .Where(x => ids.Contains(x.Id) || ids.Contains(x.ParentId ?? 0))
            .Select(x => x.Id)
            .ToList();

        return _dbContext.FolderLog
            .Where(x => folderIds.Contains(x.Id) || folderIds.Contains(x.ParentId ?? 0));
    }

    public IQueryable<FolderLog> GetByName(string name)
    {
        return _dbContext.FolderLog.Where(m => m.Name.ToLower().Equals(name.ToLower()));
    }
}