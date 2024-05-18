namespace ERPSystem.DataModel.API;

public class JwtOptionsModel
{
    public string SecretKey { get; set; }
    public int ExpiryMinutes { get; set; }
    public int ExpiryRefreshToken { get; set; }
    public string Issuer { get; set; }
    public bool ValidateLifetime { get; set; }
}