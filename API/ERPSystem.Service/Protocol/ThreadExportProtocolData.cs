using ERPSystem.DataModel.MeetingLog;

namespace ERPSystem.Service.Protocol
{
    public class ThreadExportProtocolData : ProtocolData<ExportToFileDetailData>
    {
        public string Type { get; set; }
    }

    public class ExportToFileDetailData
    {
        public Room Room { get; set; }
        public int UserId { get; set; }
        public Action Action { get; set; }

    }

    public class Room
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRunning { get; set; }
        public int TotalPeople { get; set; }
        public int CurrentPeople { get; set; }
        public double Price { get; set; }
        public List<int> UserListId { get; set; }

    }
    public class Action
    { 
        public int Id { get; set; }
        public int MeetingLogId { get; set; }
        public bool CreateBattleSuccess { get; set; }
        public bool DepositDone { get; set; }
        public bool DoneReceiveReward { get; set; }
        public List<ChatInfo> Content { get; set; }
        public List<UserData> UserStatus { get; set; }
        public GamePlay GamePlay { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public string UserId1Name { get; set; }
        public string UserId2Name { get; set; }
  
    }
    public class GamePlay
    { 
        public List<Card> CardList { get; set; }
        public int OldTurnId { get; set; }
        public int CurrentTurnId { get; set; }
        public int NextTurnId { get; set; }
        public string ItemNFTId { get; set; }
        public List<string> CardId { get; set; }
        public List<int> OrderWinning { get; set; }
    }
    public class ChatInfo
    { 
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public string Avatar { get; set; }
        public List<ReactInfo> Reactions { get; set; }
    }
    public class ReactInfo
    { 
        public int Id { get; set; }
        public int Quantity { get; set; }

    }
    public class Card
    { 
        public string Id { get; set; }
        public string Image { get; set; }
        public string Deck { get; set; }
        public bool Choose { get; set; }
        public int UserId { get; set; }

    }
    
}


