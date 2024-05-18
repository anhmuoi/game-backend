namespace ERPSystem.DataAccess.Models;

public class WorkLog : Base
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int FolderLogId { get; set; }
    public FolderLog FolderLog { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}