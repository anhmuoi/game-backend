using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ERPSystem.Common.Resources;
using Newtonsoft.Json;
using ERPSystem.Common.Infrastructure;

namespace ERPSystem.DataModel.Account
{
    public class AccountModel
    {
        //[JsonIgnore]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public short Type { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreateDateRefreshToken { get; set; }
        public bool IsDeleted { get; set; }
    
    }
    public class AccountDataModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int CompanyId { get; set; }
        public bool RootFlag { get; set; }
        public int Role { get; set; }
        public short Status { get; set; }
        public string TimeZone { get; set; }
        public bool IsCurrentAccount { get; set; }
        public IEnumerable<EnumModel> RoleList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }

    public class AccountListModelInit
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public class AccountListModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        
        public string FirstName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public List<string> CompanyNames { get; set; }
        public string TimeZone { get; set; }
        public string Department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        //public string Remarks { get; set; }
    }

    //public class ForgotPasswordModel
    //{
    //    public string CompanyCode { get; set; }
    //    public string Username { get; set; }
    //}

    public class ContactModel
    {
        public int CompanyId { get; set; }
        public string Contact { get; set; }
    }

    public class AccountAvatarModel
    {
        public string Avatar { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
