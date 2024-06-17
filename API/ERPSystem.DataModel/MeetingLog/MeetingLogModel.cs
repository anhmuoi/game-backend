namespace ERPSystem.DataModel.MeetingLog;

public class MeetingLogModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int? MeetingRoomId { get; set; }
    
    public bool CreateBattleSuccess { get; set; }
    public bool DepositDone { get; set; }
    
    public List<ChatInfo> Content { get; set; }
    public List<UserData> UserList { get; set; }
    public GamePlay GamePlay { get; set; }

}
public class MeetingLogResponseModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int FolderLogId { get; set; }
    public int? MeetingRoomId { get; set; }
    public bool IsAction { get; set; }
    public bool CreateBattleSuccess { get; set; }
    public bool DepositDone { get; set; }

    public List<ChatInfo> Content { get; set; }
    public List<UserData> UserList { get; set; }
    public GamePlay GamePlay { get; set; }

}
public class UserData
{
    public int Id { get; set; }
    public string WalletAddress { get; set; }
    public string Avatar { get; set; }
    public string Name { get; set; }
    public int Hp { get; set; }
    public int Mana { get; set; }
    public int Shield { get; set; }
    public int IndexPlayer { get; set; }
    public string skillId { get; set; }
    public bool IsDeposit { get; set; }


    public List<NFT> NFTList { get; set; }
}
public class NFT
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsUse { get; set; }
    public int Index { get; set; }
    public string Image { get; set; }
    public int Mana { get; set; }

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