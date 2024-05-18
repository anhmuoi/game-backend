namespace ERPSystem.DataModel.Login;

public class ChangePasswordModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}