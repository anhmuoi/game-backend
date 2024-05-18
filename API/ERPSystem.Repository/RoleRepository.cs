using System.Globalization;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Role;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ERPSystem.Repository;

public interface IRoleRepository : IGenericRepository<Role>
{
    void AddDefaultRole();
    Role GetById(int roleId);
    Role GetByName(string name);
    RoleModel GetByAccountId(int accountId);

    PermissionGroupModel GetListModel(KeyValuePair<string, Dictionary<string, bool>> data, CultureInfo culture,
        int accountType = (int)RoleType.DynamicRole);
}

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext,
        contextAccessor)
    {
        _dbContext = dbContext;
    }

    public Role GetById(int roleId)
    {
        return _dbContext.Role.FirstOrDefault(x => x.Id == roleId && !x.IsDeleted);
    }

    public Role GetByName(string name)
    {
        return _dbContext.Role.FirstOrDefault(m => m.Name.ToLower() == name.ToLower() && !m.IsDeleted);
    }

    public RoleModel GetByAccountId(int accountId)
    {
        var role = (from a in _dbContext.Role
            join b in _dbContext.Account on a.Id equals b.RoleId
            where !a.IsDeleted && b.Id == accountId
            select new RoleModel
            {
                Id = a.Id,
                RoleName = a.Name,
                IsDefault = a.Type != (int)RoleType.DynamicRole,
                PermissionGroups = JsonConvert.DeserializeObject<List<PermissionGroupModel>>(a.PermissionList)
            }).FirstOrDefault();

        return role;
    }

    public void AddDefaultRole()
    {
        var defualtRole = _dbContext.Role.FirstOrDefault(x => x.Type == (short)RoleType.PrimaryManager && !x.IsDeleted);
        if (defualtRole == null)
        {
            Role primary = new Role()
            {
                Type = (short)RoleType.PrimaryManager,
                IsDefault = true,
                IsDeleted = false,
                Name = RoleType.PrimaryManager.GetDescription(),
                PermissionList = ChangeModelToJSON(SetDefaultRoleValue((int)RoleType.PrimaryManager))
            };
            Add(primary);
        }
        else
        {
            var permission = JsonConvert.DeserializeObject<List<PermissionGroupDataModel>>(defualtRole.PermissionList) 
                             ?? new List<PermissionGroupDataModel>();
            var currentPermission = SetDefaultRoleValue((int)RoleType.PrimaryManager);
            if (permission.Count != currentPermission.Count)
            {
                defualtRole.PermissionList = ChangeModelToJSON(currentPermission);
                Update(defualtRole);
            }
        }
    }

    /// <summary>
    /// This function changes model data to JSON string form data.
    /// This function is used to store data in DB without name and description.
    /// </summary>
    /// <param name="permissionDataGroups"></param>
    /// <returns></returns>
    private string ChangeModelToJSON(List<PermissionGroupDataModel> permissionDataGroups)
    {
        var permissionList = JsonConvert.SerializeObject(permissionDataGroups, Formatting.None);

        return permissionList;
    }

    private List<PermissionGroupDataModel> SetDefaultRoleValue(int accountType = (int)RoleType.DynamicRole, string currentPermissions = null)
    {
        var permissionGroups = new List<PermissionGroupDataModel>();

        foreach (var eachData in PagePermission.GetAllPermssions())
        {
            permissionGroups.Add(GetListModelforDB(eachData, accountType, currentPermissions));
        }

        return permissionGroups;
    }

    /// <summary>
    /// Get data to store in DB. ( Compare version )
    /// </summary>
    /// <param name="data"> role and permission data in Key-Value pair form </param>
    /// <param name="accountType"> integer flag to distinguish that account is primary manager or employee or not(dynamic) </param>
    /// <returns></returns>
    public PermissionGroupDataModel GetListModelforDB(KeyValuePair<string, Dictionary<string, bool>> data,
        int accountType = (int)RoleType.DynamicRole, string currentPermissions = null)
    {
        var permissionGroup = new PermissionGroupDataModel();

        var permissions = new List<PermissionDataModel>();

        foreach (var permission in data.Value)
        {
            var permissionModel = new PermissionDataModel
            {
                Title = permission.Key,
                IsEnabled = accountType == (int)RoleType.DynamicRole && string.IsNullOrEmpty(currentPermissions)
                    ? permission.Value
                    : IsEnabledByAccount(accountType, permission, currentPermissions)
            };
            permissions.Add(permissionModel);
        }

        permissionGroup.Title = data.Key;
        permissionGroup.Permissions = permissions;

        return permissionGroup;
    }
    /// <summary>
    /// Get data to display on the web, including translated sentences.
    /// </summary>
    /// <param name="data"> role and permission data in Key-Value pair form </param>
    /// <param name="culture"> culture information for localizaion </param>
    /// <param name="accountType"> integer flag to distinguish that account is primary manager or employee or not(dynamic) </param>
    /// <param name="forDB"> boolean flag to distinguish that data is for on web or to DB. </param>
    /// <returns></returns>
    public PermissionGroupModel GetListModel(KeyValuePair<string, Dictionary<string, bool>> data, CultureInfo culture, int accountType = (int)RoleType.DynamicRole)
    {
        var permissionGroup = new PermissionGroupModel();

        var permissions = new List<PermissionModel>();

        foreach (var permission in data.Value)
        {
            var permissionModel = new PermissionModel
            {
                Title = permission.Key,
                PermissionName = PermissionResource.ResourceManager.GetString(permission.Key, culture),
                Description = PermissionResource.ResourceManager.GetString(Constants.Description + permission.Key, culture),
                IsEnabled = accountType == (int)RoleType.DynamicRole
                    ? permission.Value
                    : IsEnabledByAccount(accountType, permission)
            };
            permissions.Add(permissionModel);
        }
        permissionGroup.Title = data.Key;
        permissionGroup.GroupName = PermissionResource.ResourceManager.GetString(data.Key, culture);
        permissionGroup.Permissions = permissions;

        return permissionGroup;
    }

    private bool IsEnabledByAccount(int accountType, KeyValuePair<string, bool> permission,
        string currentPermission = null)
    {
        switch (accountType)
        {
            case (int)RoleType.PrimaryManager:
                return true;
            case (int)RoleType.DynamicRole:
                return IsDynamicEnabled(permission.Key, permission.Value, currentPermission);
            default:
                return false;
        }
    }

    private bool IsDynamicEnabled(string permissionName, bool permissionValue, string currentPermission)
    {
        var permission = JsonConvert.DeserializeObject<List<PermissionGroupDataModel>>(currentPermission)
            .SelectMany(m => m.Permissions)
            .Where(m => m.Title == permissionName)
            .FirstOrDefault();

        if (permission != null)
        {
            return permission.IsEnabled;
        }
        else
        {
            return permissionValue;
        }
    }
}