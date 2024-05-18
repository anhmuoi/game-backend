using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Repository;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    new IQueryable<Department> Gets();
    Department? GetByNumber(string number);
    Department? GetByName(string name);
}

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    private readonly AppDbContext _dbContext;
    public DepartmentRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }
    
    public override IQueryable<Department> Gets()
    {
        return _dbContext.Department
            .Include(m => m.DepartmentManager).ThenInclude(m => m!.User)
            .Include(m => m.Parent);
    }
    
    public Department? GetByNumber(string number)
    {
        return _dbContext.Department.FirstOrDefault(m => m.Number.ToLower() == number.ToLower());
    }

    public Department? GetByName(string name)
    {
        return _dbContext.Department.FirstOrDefault(m => m.Name.ToLower() == name.ToLower());
    }
}