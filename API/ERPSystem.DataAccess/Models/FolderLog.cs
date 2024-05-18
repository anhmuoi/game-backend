using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class FolderLog : Base
{
    public FolderLog()
    {
        WorkSchedule = new HashSet<WorkSchedule>();
        DailyReport = new HashSet<DailyReport>();
        ChildFolderLog = new HashSet<FolderLog>();
        WorkLog = new HashSet<WorkLog>();
    }

    public int Id { get; set; }
    public int? ParentId { get; set; }
    public FolderLog? Parent { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<WorkSchedule> WorkSchedule { get; set; }
    public ICollection<DailyReport> DailyReport { get; set; }
    public ICollection<FolderLog> ChildFolderLog { get; set; }
    public ICollection<WorkLog> WorkLog { get; set; }
}