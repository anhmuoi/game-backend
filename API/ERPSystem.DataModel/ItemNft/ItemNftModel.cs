using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ERPSystem.DataModel.ItemNft
{
    public class ItemNftModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }

        public string CreatedOn { get; set; }

    }

    public class ItemNftEditModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }
    }

    public class ItemNftAddModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Mana { get; set; }
        public string AliasId { get; set; }
        public string CreatedOn { get; set; }
    }

    public class ItemNftListModel
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
    }

    

}
