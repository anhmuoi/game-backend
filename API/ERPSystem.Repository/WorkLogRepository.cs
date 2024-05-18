using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IWorkLogRepository : IGenericRepository<WorkLog>
{
    IQueryable<WorkLog> GetByIds(List<int> ids);
    IQueryable<WorkLog> GetByFolderLogIds(List<int> folderLogIds, string search = "");
    IQueryable<WorkLog> GetByFolderLogId(int folderId);
}

public class WorkLogRepository : GenericRepository<WorkLog>, IWorkLogRepository
{
    private readonly AppDbContext _dbContext;
    public WorkLogRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public IQueryable<WorkLog> GetByIds(List<int> ids)
    {
        return _dbContext.WorkLog.Where(x => ids.Contains(x.Id));
    }
    public IQueryable<WorkLog> GetByFolderLogIds(List<int> folderLogIds, string search)
    {
        var listWorkLog = _dbContext.WorkLog.Where(m => folderLogIds.Contains(m.FolderLogId));
        if (!string.IsNullOrEmpty(search))
        {
            return listWorkLog.Where(x => x.Title.ToLower().Contains(search.ToLower()) || x.Content.Contains(search));
        }

        return listWorkLog;
    }
    public IQueryable<WorkLog> GetByFolderLogId(int folderId)
    {
        return _dbContext.WorkLog.Where(m => m.FolderLogId == folderId);
    }
}