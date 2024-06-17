using System.Data.Entity;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IItemNftUserRepository : IGenericRepository<ItemNftUser>
{
    IQueryable<ItemNftUser> Gets();
    ItemNftUser GetById(int userId);
    public void AddItemNftUser(int itemNftId, int userId, double price);

}

public class ItemNftUserRepository : GenericRepository<ItemNftUser>, IItemNftUserRepository
{
    private readonly AppDbContext _dbContext;
    public ItemNftUserRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

 
    public IQueryable<ItemNftUser> Gets()
    {
        return _dbContext.ItemNftUser.Include(m => m.User);
    }
    public ItemNftUser GetById(int id)
    {
        var itemNftUser = _dbContext.ItemNftUser.FirstOrDefault(n=>n.Id == id);
        return itemNftUser;
    }

    public void AddItemNftUser(int itemNftId, int userId, double price)
    {
        var itemNft = _dbContext.ItemNft.FirstOrDefault(m => m.Id == itemNftId);
        _dbContext.ItemNftUser.Add(new ItemNftUser()
        {
            Address = itemNft.Address,
            Description = itemNft.Description,
            Name = itemNft.Name,
            Image = itemNft.Image,
            IsUse = false,
            AliasId = itemNft.AliasId,
            Mana = itemNft.Mana,
            Price = price,
            Status = 0,
            UserId = userId,
            ItemNftId = itemNftId,
            CreatedOn = DateTime.UtcNow,
        });

        _dbContext.SaveChanges();
    }
   
}