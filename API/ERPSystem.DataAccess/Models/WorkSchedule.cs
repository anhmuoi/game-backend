using System;

namespace ERPSystem.DataAccess.Models;

public class WorkSchedule : Base
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public Category Category { get; set; }
    public int? FolderLogId { get; set; }
    public FolderLog FolderLog { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public short Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}