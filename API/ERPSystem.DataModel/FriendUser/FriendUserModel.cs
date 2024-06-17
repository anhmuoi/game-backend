using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ERPSystem.DataModel.FriendUser
{
    public class FriendUserModel
    {
        public int Id { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }

        public string CreatedOn { get; set; }

    }

    public class FriendUserEditModel
    {
        public int Id { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
    }

    public class FriendUserAddModel
    {
        public int Id { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public string CreatedOn { get; set; }
    }

    public class FriendUserListModel
    {
        public int Id { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
    }

    

}
