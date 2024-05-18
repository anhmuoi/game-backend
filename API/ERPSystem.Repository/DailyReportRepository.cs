using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IDailyReportRepository : IGenericRepository<DailyReport>
{
    IQueryable<DailyReport> GetByIds(List<int> ids);
    IQueryable<DailyReport> GetByFolderLogIds(List<int> folderLogIds, string search = "");
    IQueryable<DailyReport> GetByFolderLogId(int folderId);
}

public class DailyReportRepository : GenericRepository<DailyReport>, IDailyReportRepository
{
    private readonly AppDbContext _dbContext;
    public DailyReportRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public IQueryable<DailyReport> GetByIds(List<int> ids)
    {
        return _dbContext.DailyReport.Where(x => ids.Contains(x.Id));
    }
    public IQueryable<DailyReport> GetByFolderLogIds(List<int> folderLogIds, string search)
    {
        var listDailyReport = _dbContext.DailyReport.Where(m => folderLogIds.Contains(m.FolderLogId.Value));
        if (!string.IsNullOrEmpty(search))
        {
            return listDailyReport.Where(x => x.Title.ToLower().Contains(search.ToLower()) || x.Content.Contains(search));
        }

        return listDailyReport;
    }
    public IQueryable<DailyReport> GetByFolderLogId(int folderId)
    {
        return _dbContext.DailyReport.Where(m => m.FolderLogId == folderId);
    }
}