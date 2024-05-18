using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Department : Base
{
    public Department()
    {
        User = new HashSet<User>();
        ChildDepartment = new HashSet<Department>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int? DepartmentManagerId { get; set; }
    public Account? DepartmentManager { get; set; }
    public int? ParentId { get; set; }
    public Department? Parent { get; set; }
    public ICollection<User> User { get; set; }
    public ICollection<Department> ChildDepartment { get; set; }
}