
using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class ItemNftUser : Base
{

    public int Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string AliasId { get; set; }
    public bool IsUse { get; set; }
    public string Image { get; set; }
    public int Mana { get; set; }
    public double Price { get; set; }
    public int Status { get; set; }


    public int UserId { get; set; }
    public User User { get; set; }

    
    public int ItemNftId { get; set; }
    public ItemNft ItemNft { get; set; }


}