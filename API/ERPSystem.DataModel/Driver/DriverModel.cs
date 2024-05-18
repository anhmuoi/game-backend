namespace ERPSystem.DataModel.Driver;

public class DriverShareModel
{
    public int PermissionId { get; set; }
    public List<int> UserIds { get; set; }
}

public class DocumentListModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? FolderId { get; set; }
    public int? FileId { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAvatar { get; set; }
    public string UpdatedOn { get; set; }
    public string CreatedOn { get; set; }
    public long Size { get; set; }
    public bool Shared { get; set; }
    public bool CanBeEdit { get; set; }
}

public class DriverFilterModel : FilterModel
{
    public int FolderId { get; set; }
    public int FileId { get; set; }
    public string Name { get; set; }
}

public class UserShareModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public int PermissionType { get; set; }
    public string Permission { get; set; }
    public string Email { get; set; }
    public string DepartmentName { get; set; }
}