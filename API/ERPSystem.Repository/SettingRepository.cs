using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface ISettingRepository : IGenericRepository<Setting>
{
    
}

public class SettingRepository : GenericRepository<Setting>, ISettingRepository
{
    private readonly AppDbContext _dbContext;
    public SettingRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
}