using Microsoft.AspNetCore.Http;

namespace ERPSystem.DataModel.Driver;

public class FileAddModel
{
    public int? FolderId { get; set; }
    public IFormFile File { get; set; }
}

public class FileEditModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}