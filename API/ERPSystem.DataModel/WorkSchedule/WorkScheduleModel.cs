namespace ERPSystem.DataModel.WorkSchedule;

public class WorkScheduleModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int? FolderLogId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public short Type { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool IsAllDay { get; set; }
}

public class WorkScheduleFilterModel : FilterModel
{
    public string Title { get; set; }
    public int CategoryId { get; set; }
    public int FolderLogId { get; set; }
    public List<int>? Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
public class WorkScheduleListModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int FolderLogId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public short Type { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int CreatedBy { get; set; }
}