namespace ERPSystem.DataModel.Setting;

public class CouponSettingModel
{
    public WelcomeCouponSettingModel? WelcomeCoupon { get; set; }
    public VisitCouponSettingModel? VisitCoupon { get; set; }
    public MemberLevelCouponSettingModel? MemberLevel { get; set; }
    public BirthdayCouponSettingModel? BirthdayCoupon { get; set; }
}

public class WelcomeCouponSettingModel
{
    public List<int> CouponId { get; set; }
}

public class VisitCouponSettingModel
{
    public int CouponId { get; set; }
    public int VisitedTime { get; set; }
}

public class MemberLevelCouponSettingModel
{
    public int CouponId { get; set; }
    public double Point { get; set; }
}

public class BirthdayCouponSettingModel
{
    public int CouponId { get; set; }
}