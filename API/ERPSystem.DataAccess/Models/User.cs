using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class User : Base
{
    public User()
    {
        UserFile = new HashSet<UserFile>();
        ItemNftUser = new HashSet<ItemNftUser>();
        DailyReport = new HashSet<DailyReport>();
        WorkLog = new HashSet<WorkLog>();
        UserFolder = new HashSet<UserFolder>();
    }
    public int Id { get; set; }
    public string WalletAddress { get; set; }
    public string Avatar { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Position { get; set; }
    public short Status { get; set; }
    public int? AccountId { get; set; }
    public int InGame { get; set; }
    public int OwnerRoom { get; set; }
    public int IndexPlayer { get; set; }
    public double Balance { get; set; }
    public int TotalWin { get; set; }
    public int TotalLose { get; set; }
    public Account? Account { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public ICollection<UserFile> UserFile { get; set; }
    public ICollection<DailyReport> DailyReport { get; set; }
    public ICollection<WorkLog> WorkLog { get; set; }
    public ICollection<UserFolder> UserFolder { get; set; }
    public ICollection<ItemNftUser> ItemNftUser { get; set; }
}