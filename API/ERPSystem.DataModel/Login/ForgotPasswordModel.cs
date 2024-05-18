using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Resources;

namespace ERPSystem.DataModel.Login;

public class ForgotPasswordModel
{
    [Display(Name = nameof(AccountResource.lblEmail), ResourceType = typeof(AccountResource))]
    public string Email { get; set; }
}