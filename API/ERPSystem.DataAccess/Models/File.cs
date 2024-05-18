using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class File : Base
{
    public File()
    {
        UserFile = new HashSet<UserFile>();
    }

    public int Id { get; set; }
    public int? FolderId { get; set; }
    public Folder? Folder { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public long Size { get; set; }
    public ICollection<UserFile> UserFile { get; set; }
}