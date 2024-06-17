using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class FriendUser : Base
{
   
    public int Id { get; set; }
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
   
}