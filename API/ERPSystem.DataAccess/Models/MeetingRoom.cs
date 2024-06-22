using System.Collections.Generic;
using Newtonsoft.Json;

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
    public string PasswordRoom { get; set; }
    public bool IsRunning { get; set; }
    public bool IsDelete { get; set; }
    public bool Default { get; set; }
    public int TotalPeople { get; set; }
    public int CurrentPeople { get; set; }
    public int CurrentMeetingLogId { get; set; }
    public double Price { get; set; }
    public string? UserListId { get; set; }

    [JsonIgnore]
    public ICollection<MeetingLog> MeetingLog { get; set; }
}