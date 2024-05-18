namespace ERPSystem.DataModel.Category;

public class CategoryFilterModel : FilterModel
{
    public string Name { get; set; }
    public List<int>? Types { get; set; }
}