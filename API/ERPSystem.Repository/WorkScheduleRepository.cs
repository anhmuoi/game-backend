using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IWorkScheduleRepository : IGenericRepository<WorkSchedule>
{
    IQueryable<WorkSchedule> GetByIds(List<int> ids);
    IQueryable<WorkSchedule> GetByFolderLogIds(List<int> folderIds, string search = "");
    IQueryable<WorkSchedule> GetByFolderLogId(int folderId);
}

public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
{
    private readonly AppDbContext _dbContext;
    public WorkScheduleRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public IQueryable<WorkSchedule> GetByIds(List<int> ids)
    {
        return _dbContext.WorkSchedule.Where(m => ids.Contains(m.Id));
    }

    public IQueryable<WorkSchedule> GetByFolderLogIds(List<int> folderIds, string search)
    {
        var listWorkSchedule = _dbContext.WorkSchedule.Where(m => folderIds.Contains(m.FolderLogId ?? 0));
        if (!string.IsNullOrEmpty(search))
        {
            return listWorkSchedule.Where(x => x.Title.ToLower().Contains(search.ToLower()) || x.Content.Contains(search));
        }

        return listWorkSchedule;
    }

    public IQueryable<WorkSchedule> GetByFolderLogId(int folderId)
    {
        return _dbContext.WorkSchedule.Where(m => m.FolderLogId == folderId);
    }
}