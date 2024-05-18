using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class MeetingRoom : Base
{
    public MeetingRoom()
    {
        MeetingLog = new HashSet<MeetingLog>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsRunning { get; set; }
    public int TotalPeople { get; set; }
    public int CurrentPeople { get; set; }
    public double Price { get; set; }
    public string? UserListId { get; set; }
    public ICollection<MeetingLog> MeetingLog { get; set; }
}