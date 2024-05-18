using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IMeetingLogRepository : IGenericRepository<MeetingLog>
{
    IQueryable<MeetingLog> GetByIds(List<int> ids);
}

public class MeetingLogRepository : GenericRepository<MeetingLog>, IMeetingLogRepository
{
    private readonly AppDbContext _dbContext;
    public MeetingLogRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }


    public IQueryable<MeetingLog> GetByIds(List<int> ids)
    {
        return _dbContext.MeetingLog.Where(x => ids.Contains(x.Id));
    }
}