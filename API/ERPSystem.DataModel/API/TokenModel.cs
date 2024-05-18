namespace ERPSystem.DataModel.API;

public class TokenModel
{
    public int Status { get; set; }
    public string AuthToken { get; set; }

    public string RefreshToken { get; set; }

    public string DepartmentName { get; set; }
    
    public string UserTimeZone { get; set; }
    public string UserLanguage { get; set; }
    public Dictionary<string, string> QueueService { get; set; }
    
    public int? AccountId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public int ExpireAccessToken { get; set; }
    public string Role { get; set; }
    public Dictionary<string, Dictionary<string,bool>> Permissions { get; set; }
}

public class TokenRefreshModel
{
    public string ExpiredToken { get; set; }

    public string RefreshToken { get; set; }
}