using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Category : Base
{
    public Category()
    {
        WorkSchedule = new HashSet<WorkSchedule>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public short Type { get; set; }
    public string Color { get; set; }

    public ICollection<WorkSchedule> WorkSchedule { get; set; }
}