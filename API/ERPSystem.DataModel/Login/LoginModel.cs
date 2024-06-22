using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ERPSystem.DataModel.Login;

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
public class LoginByAddressModel
{
    public string Address { get; set; }
    public double Balance { get; set; }
}