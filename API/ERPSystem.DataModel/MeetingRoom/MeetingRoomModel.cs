namespace ERPSystem.DataModel.MeetingRoom;

public class MeetingRoomModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PasswordRoom { get; set; }
    public bool Default { get; set; }
    public bool IsRunning { get; set; }
    public int TotalPeople { get; set; }
    public int CurrentPeople { get; set; }
    public int CurrentMeetingLogId { get; set; }
    public double Price { get; set; }
    public List<int> UserListId { get; set; }
}
public class MeetingRoomResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PasswordRoom { get; set; }
    public bool Default { get; set; }
    public bool IsRunning { get; set; }
    public int TotalPeople { get; set; }
    public int CurrentPeople { get; set; }
    public int CurrentMeetingLogId { get; set; }
    public double Price { get; set; }
    public List<int> UserListId { get; set; }
}
