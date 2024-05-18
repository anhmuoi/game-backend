using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IMeetingRoomRepository : IGenericRepository<MeetingRoom>
{
    IQueryable<MeetingRoom> GetByIds(List<int> ids);
}

public class MeetingRoomRepository : GenericRepository<MeetingRoom>, IMeetingRoomRepository
{
    private readonly AppDbContext _dbContext;
    public MeetingRoomRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
    public IQueryable<MeetingRoom> GetByIds(List<int> ids)
    {
        return _dbContext.MeetingRoom.Where(x => ids.Contains(x.Id));
    }
}