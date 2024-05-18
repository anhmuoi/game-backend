using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Category? GetByName(string name);
}

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _dbContext;
    public CategoryRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public Category? GetByName(string name)
    {
        return _dbContext.Category.FirstOrDefault(m => m.Name.ToLower() == name.ToLower());
    }
}