using System.Data.Entity;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IAccountRepository : IGenericRepository<Account>
{
    // Account GetByUserName(string username);
    Account AddDefaultAccount(string userName, string password);
    string GetRefreshTokenByUserName(string username);
    void AddRefreshToken( string refreshToken, Account model);
    Account GetByUserId(int id);
    IQueryable<Account> GetByRoleId(int roleId);
    IQueryable<Account> GetByRoleIds(List<int> roleIds);
}

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    private readonly AppDbContext _dbContext;
    public AccountRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

    public string GetRefreshTokenByUserName(string username)
    {
        return _dbContext.Account.First(x => x.UserName.ToLower() == username.ToLower())?.RefreshToken;
    }

    public void AddRefreshToken(string refreshToken, Account model)
    {
        try
        {
            var accountLogin = _dbContext.Account.Where(x => x.UserName.ToLower() == model.UserName.ToLower())
                .Select(x => x.Id).FirstOrDefault();
            Account acc = _dbContext.Account.FirstOrDefault(item => item.Id == accountLogin);
            if (acc != null)
            {
                acc.RefreshToken = refreshToken;
                _dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }

    public Account GetByUserId(int id)
    {
        return _dbContext.Account.Include(m => m.User).FirstOrDefault(n => n.User.Id == id);
    }

    public IQueryable<Account> GetByRoleId(int roleId)
    {
        var accounts = _dbContext.Account.Include(x => x.Role).Where(x => x.Role.Id == roleId && !x.Role.IsDeleted);
        return accounts;
    }

    public IQueryable<Account> GetByRoleIds(List<int> roleIds)
    {
        return _dbContext.Account.Include(x => x.Role).Where(x => roleIds.Contains(x.Role.Id) && !x.Role.IsDeleted);
    }

    /// <summary>
    /// Add or update a default account
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    public Account AddDefaultAccount(string userName, string password)
    {
        var account = _dbContext.Account.FirstOrDefault();
        if (account == null)
        {
            var role = _dbContext.Role.FirstOrDefault(x => x.Type == (short)RoleType.PrimaryManager && !x.IsDeleted);
            if (role != null)
            {
                account = new Account
                {
                    UserName = userName,
                    Password = SecurePasswordHasher.Hash(password),
                    Language = "en-US",
                    Timezone = "Etc/UTC",
                    RoleId = role.Id,
                };
                Add(account);
            }
            
        }
        else
        {
            if (!string.IsNullOrEmpty(password))
            {
                account.Password = SecurePasswordHasher.Hash(password);
            }
            account.UserName = userName;
        }

        return account;
    }
}