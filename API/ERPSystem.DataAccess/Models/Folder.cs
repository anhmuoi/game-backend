using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class Folder : Base
{
    public Folder()
    {
        ChildFolder = new HashSet<Folder>();
        File = new HashSet<File>();
        UserFolder = new HashSet<UserFolder>();
    }

    public int Id { get; set; }
    public int? ParentId { get; set; }
    public Folder? Parent { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Folder> ChildFolder { get; set; }
    public ICollection<File> File { get; set; }
    public ICollection<UserFolder> UserFolder { get; set; }
}