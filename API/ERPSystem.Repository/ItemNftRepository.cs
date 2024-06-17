using System.Data.Entity;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IItemNftRepository : IGenericRepository<ItemNft>
{
    IQueryable<ItemNft> Gets();
    ItemNft GetById(int userId);

}

public class ItemNftRepository : GenericRepository<ItemNft>, IItemNftRepository
{
    private readonly AppDbContext _dbContext;
    public ItemNftRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
    {
        _dbContext = dbContext;
    }

 
    public IQueryable<ItemNft> Gets()
    {
        return _dbContext.ItemNft;
    }
    public ItemNft GetById(int id)
    {
        var itemNft = _dbContext.ItemNft.FirstOrDefault(n=>n.Id == id);
        return itemNft;
    }
   
}