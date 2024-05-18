using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    
}

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    private readonly AppDbContext _dbContext;
    public SupplierRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
}