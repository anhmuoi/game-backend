using System;

namespace ERPSystem.DataAccess.Models;

public class MeetingLog : Base
{
    public int Id { get; set; }
    public int? MeetingRoomId { get; set; }
    public MeetingRoom MeetingRoom { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string UserList { get; set; }

}