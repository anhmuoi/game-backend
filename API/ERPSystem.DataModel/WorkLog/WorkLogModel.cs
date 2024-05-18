namespace ERPSystem.DataModel.WorkLog;

public class WorkLogModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int UserId { get; set; }
}
public class WorkLogResponseModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int UserId { get; set; }
    public bool IsAction { get; set; }
}