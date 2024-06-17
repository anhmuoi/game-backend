using System.Data.Entity;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IBalanceHistoryRepository : IGenericRepository<BalanceHistory>
{
    IQueryable<BalanceHistory> Gets();
    BalanceHistory GetById(int userId);

}

public class BalanceHistoryRepository : GenericRepository<BalanceHistory>, IBalanceHistoryRepository
{
    private readonly AppDbContext _dbContext;
    public BalanceHistoryRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

 
    public IQueryable<BalanceHistory> Gets()
    {
        return _dbContext.BalanceHistory;
    }
    public BalanceHistory GetById(int id)
    {
        var balanceHistory = _dbContext.BalanceHistory.FirstOrDefault(n=>n.Id == id);
        return balanceHistory;
    }
   
}