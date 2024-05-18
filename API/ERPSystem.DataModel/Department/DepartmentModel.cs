namespace ERPSystem.DataModel.Department;

public class DepartmentAddModel
{
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public int? ParentId { get; set; }
}

public class DepartmentEditModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public int? ParentId { get; set; }
}

public class DepartmentListModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public string? DepartmentManager { get; set; }
    public int? ParentId { get; set; }
    public string? DepartmentParent { get; set; }
}

public class DepartmentModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public string? DepartmentManager { get; set; }
    public int? ParentId { get; set; }
    public string? DepartmentParent { get; set; }
}

public class DepartmentItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public string? DepartmentManager { get; set; }
    public int? ParentId { get; set; }
    public string? DepartmentParent { get; set; }
    public List<DepartmentItemModel> Children { get; set; }
}