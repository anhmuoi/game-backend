using System;

namespace ERPSystem.DataAccess.Models;

public class Base
{
    public DateTime CreatedOn { get; set; }
    public int CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public int UpdatedBy { get; set; }
    public Base()
    {
        CreatedOn = DateTime.UtcNow;
        UpdatedOn = DateTime.UtcNow;
    }
}