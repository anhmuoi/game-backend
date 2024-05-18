namespace ERPSystem.DataModel.FolderLog;

public class FolderLogModel
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
public class FolderLogListModel
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UpdatedBy { get; set; }
    public string UpdatedOn { get; set; }
    public List<FolderLogListModel> Children { get; set; }
    public List<ChildFolderModel> Schedule { get; set; }
    public List<ChildFolderModel> Meeting { get; set; }
    public List<ChildFolderModel> DailyReport { get; set; }
    public List<ChildFolderModel> WorkLog { get; set; }
}

public class FolderLogSearch
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class ChildFolderModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int UpdatedBy { get; set; }
    public string UpdatedOn { get; set; }
}