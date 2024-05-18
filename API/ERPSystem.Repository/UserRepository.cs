using System.Data.Entity;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    User? GetUserByAccountId(int accountId);
    IQueryable<User> Gets();
    User GetById(int userId);
    User? GetByPhone(string phone);
    User? GetByUserName(string name);
}

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    public UserRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public User? GetUserByAccountId(int accountId)
    {
        var user = _dbContext.User.FirstOrDefault(x => x.AccountId == accountId);
        return user;
    }
    public IQueryable<User> Gets()
    {
        return _dbContext.User;
    }
    public User GetById(int id)
    {
        var user = _dbContext.User.Include(m => m.Account).FirstOrDefault(n=>n.Id == id);
        return user;
    }
    public User? GetByName(string name)
    {
        return !string.IsNullOrEmpty(name) ? _dbContext.User.FirstOrDefault(m => m.Name == name) : null;
    }
    public User? GetByUserName(string name)
    {
        return !string.IsNullOrEmpty(name) ? _dbContext.User.Include(m=>m.Account).FirstOrDefault(m => m.Account.UserName == name) : null;
    }
    public User? GetByPhone(string phone)
    {
        return !string.IsNullOrEmpty(phone) ? _dbContext.User.FirstOrDefault(m => m.Phone == phone) : null;
    }
}