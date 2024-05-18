namespace ERPSystem.DataModel.MeetingLog;

public class MeetingLogModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int? MeetingRoomId { get; set; }
    public List<UserData> UserList { get; set; }

}
public class MeetingLogResponseModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int? MeetingRoomId { get; set; }
    public bool IsAction { get; set; }
    public List<UserData> UserList { get; set; }

}
public class UserData
{
    public int Id { get; set; }
    public string WalletAddress { get; set; }
    public List<NFT> NFTList { get; set; }
}
public class NFT
{
    public int Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}