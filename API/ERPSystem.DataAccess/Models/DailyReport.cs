namespace ERPSystem.DataAccess.Models;

public class DailyReport : Base
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? ReporterId { get; set; }
    public Account Account { get; set; }
    public int? FolderLogId { get; set; }
    public FolderLog FolderLog { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}