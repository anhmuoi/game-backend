using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ERPSystem.DataModel.User
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }
        public int DepartmentId { get; set; }

        public short Status { get; set; }
        public short RoleId { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; }

        public string CreatedOn { get; set; }

        public int? AccountId { get; set; }
    }

    public class UserEditModel
    {
        public int Id { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }

        public short Status { get; set; }

        public short RoleId { get; set; }
        public int DepartmentId { get; set; }

        public string Timezone { get; set; }
        public string Language { get; set; }

        public int UpdatedBy { get; set; }
    }

    public class UserAddModel
    {
        public int Id { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public int DepartmentId { get; set; }

        public string Position { get; set; }

        public short Status { get; set; }

        public short RoleId { get; set; }

        public string Timezone { get; set; }

        public string Language { get; set; }

        public string CreatedOn { get; set; }

        public int CreatedBy { get; set; }
    }

    public class UserListModel
    {
        public int Id { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }
        public int DepartmentId { get; set; }

        public short Status { get; set; }
        public short RoleId { get; set; }

        public string CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public string UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }

        public int? AccountId { get; set; }
    }

    public class UserAvatarModel
    {
        public string Avatar { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Password { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }

    public class AccountEditModel
    {
        public int Id { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }

        public short Status { get; set; }

        public short RoleId { get; set; }
        public int DepartmentId { get; set; }

        public string Timezone { get; set; }
        public string Language { get; set; }

        public int UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
    }

}
