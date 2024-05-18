using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Account : Base
{
    public Account()
    {
        Department = new HashSet<Department>();
        DailyReport = new HashSet<DailyReport>();
    }
    
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string Timezone { get; set; }
    public string Language { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreateDateRefreshToken { get; set; }
    
    public User? User { get; set; }
    public Role Role { get; set; }
    public ICollection<Department> Department { get; set; }
    public ICollection<DailyReport> DailyReport { get; set; }

}