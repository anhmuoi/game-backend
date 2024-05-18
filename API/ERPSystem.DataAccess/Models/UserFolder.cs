namespace ERPSystem.DataAccess.Models;

public class UserFolder : Base
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int FolderId { get; set; }
    public Folder Folder { get; set; }
    public int PermissionType { get; set; }
}