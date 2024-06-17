
using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class ItemNft : Base
{
    public ItemNft()
    {
        ItemNftUser = new HashSet<ItemNftUser>();
    }

    public int Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public int Mana { get; set; }
    public string AliasId { get; set; }
    public ICollection<ItemNftUser> ItemNftUser { get; set; }
}