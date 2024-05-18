namespace ERPSystem.DataModel.Dashboard;

public class DashboardModel
{
    public int TotalMetting { get; set; }
    public int TotalSchedule { get; set; }
    public List<DashboardDataModel> Meetings { get; set; }
    public List<DashboardDataModel> Schedules { get; set; }
}

public class DashboardDataModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}