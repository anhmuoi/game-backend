using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Role : Base
{
    public Role()
    {
        Account = new HashSet<Account>();
    }
    public int Id { get; set; }
    public short Type { get; set; }
    public string Name { get; set; }
    public string PermissionList { get; set; }
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Account> Account { get; set; }
}