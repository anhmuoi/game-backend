using System.Text;

namespace ERPSystem.DataModel.Setting;

public class CryptographyConfigModel
{
    public string AesIv { get; set; }
    public string AesKey { get; set; }
    public int TimeoutQr { get; set; } // seconds

    public byte[] GetAesIvBytes()
    {
        return Encoding.UTF8.GetBytes(AesIv);
    }
    
    public byte[] GetAesKeyBytes()
    {
        return Encoding.UTF8.GetBytes(AesKey);
    }
}