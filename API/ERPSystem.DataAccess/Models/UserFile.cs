namespace ERPSystem.DataAccess.Models;

public class UserFile : Base
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int FileId { get; set; }
    public File File { get; set; }
    public int PermissionType { get; set; }
}