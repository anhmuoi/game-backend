namespace ERPSystem.DataModel.Driver;

public class FolderAddModel
{
    public int? ParentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class FolderEditModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}