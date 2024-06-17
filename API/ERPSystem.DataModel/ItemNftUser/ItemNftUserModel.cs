using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ERPSystem.DataModel.ItemNftUser
{
    public class ItemNftUserModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }

        public string CreatedOn { get; set; }
        
        public double Price { get; set; }
        public int Status { get; set; }
        
        public int ItemNftId { get; set; }
        public int UserId { get; set; }

    }

    public class ItemNftUserEditModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        
        public int ItemNftId { get; set; }
        public int UserId { get; set; }
    }

    public class ItemNftUserAddModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }
        public string CreatedOn { get; set; }

        public double Price { get; set; }
        public int Status { get; set; }
        
        public int ItemNftId { get; set; }
        public int UserId { get; set; }
    }

    public class ItemNftUserListModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }

        public double Price { get; set; }
        public int Status { get; set; }
        
        public int ItemNftId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    

}
