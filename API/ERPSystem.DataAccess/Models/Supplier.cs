using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Supplier : Base
{
    public Supplier()
    {
        PurchaseRecord = new HashSet<PurchaseRecord>();
    }
    public int Id { get; set; }
    public string MainProduct { get; set; }
    public string Name { get; set; }
    public string Remark { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string BusinessType { get; set; }
    public ICollection<PurchaseRecord> PurchaseRecord { get; set; }
}