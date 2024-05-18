namespace ERPSystem.DataAccess.Models;

public class Setting : Base
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}