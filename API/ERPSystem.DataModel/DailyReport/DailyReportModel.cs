namespace ERPSystem.DataModel.DailyReport;

public class DailyReportModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? ReporterId { get; set; }
    public int? FolderLogId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
}
public class DailyReportResponseModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? ReporterId { get; set; }
    public int? FolderLogId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public string DepartmentName { get; set; }
    public bool IsAction { get; set; }
}

public class ReporterModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}