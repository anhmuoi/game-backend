using System.Data.Entity;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IFriendUserRepository : IGenericRepository<FriendUser>
{
    IQueryable<FriendUser> Gets();
    FriendUser GetById(int userId);

}

public class FriendUserRepository : GenericRepository<FriendUser>, IFriendUserRepository
{
    private readonly AppDbContext _dbContext;
    public FriendUserRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

 
    public IQueryable<FriendUser> Gets()
    {
        return _dbContext.FriendUser;
    }
    public FriendUser GetById(int id)
    {
        var friendUser = _dbContext.FriendUser.FirstOrDefault(n=>n.Id == id);
        return friendUser;
    }
   
}