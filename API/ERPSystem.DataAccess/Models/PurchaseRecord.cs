namespace ERPSystem.DataAccess.Models;

public class PurchaseRecord : Base
{
    public int Id { get; set; }
    public int? SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }
    public int Quantity { get; set; }
    public int Status { get; set; }
}