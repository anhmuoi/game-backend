namespace ERPSystem.DataModel.Setting;

public class CollectToMemberModel
{
    public int Type { get; set; }
    public string MemberId { get; set; }
    public string CreateTimeOfWaiting { get; set; }
    public int StoreId { get; set; }
    public string StoreCode { get; set; }
    public string StoreName { get; set; }
    public string OrderId { get; set; }
    public int Point { get; set; }
    public double Amount { get; set; }
    public List<int> MemberCouponIds { get; set; }
}