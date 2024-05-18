using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IPurchaseRecordRepository : IGenericRepository<PurchaseRecord>
{
    
}

public class PurchaseRecordRepository : GenericRepository<PurchaseRecord>, IPurchaseRecordRepository
{
    private readonly AppDbContext _dbContext;
    public PurchaseRecordRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
}