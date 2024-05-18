namespace ERPSystem.DataModel.User;

public class UserFilterModel : FilterModel
{
    public string Name { get; set; }
    public List<int>? DepartmentIds { get; set; }
}