using System.Globalization;
using AutoMapper.Configuration;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Role;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ERPSystem.Service.Services;

public interface IRoleService : IPaginationService<RoleModel>
{
    void Add(RoleModel model);
    void Update(RoleModel model);
    void Delete(Role role);
    void DeleteMultiple(List<Role> roles);
    void ChangeDefaultSettingRole(int id);
    Dictionary<string, Dictionary<string, bool>> GetPermissionsByRoleId(int roleId);
    Role GetById(int roleId);
    List<Role> GetByIds(List<int> roleIds);
    List<PermissionGroupModel> GetDefaultRoleValue(int accountType = (int)RoleType.DynamicRole);
    List<PermissionGroupModel> ChangeJsonToModel(string permissionList);
    bool CheckPermissions(List<PermissionGroupModel> permissions);
    bool IsExistedName(string name, int ignoreId);
    bool CheckPermissionEnabled(string permissionName, int accountId);
}

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HttpContext _httpContext;

    public RoleService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<RoleService>();
        _httpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// Get role data in paginated form.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortDirection"></param>
    /// <param name="totalRecords"></param>
    /// <param name="recordsFiltered"></param>
    /// <returns></returns>
    public IQueryable<RoleModel> GetPaginated(string filter, int pageNumber, int pageSize, string sortColumn,
        string sortDirection,
        out int totalRecords, out int recordsFiltered)
    {
        var result = _unitOfWork.AppDbContext.Role.Where(x => !x.IsDeleted).Select(m => new RoleModel()
        {
            Id = m.Id,
            RoleName = m.Name,
            IsDefault = m.IsDefault,
            RoleType = m.Type
            //PermissionGroups = ChangeJSONtoModel(m.PermissionList)
        });

        totalRecords = result.Count();

        if (!string.IsNullOrEmpty(filter))
        {
            filter = filter.ToLower();

            result = result.Where(m => m.RoleName.ToLower().Contains(filter));
        }

        recordsFiltered = result.Count();

        result = result.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return result.AsQueryable();
    }

    /// <summary>
    /// Add a new role
    /// </summary>
    /// <param name="model"></param>
    public void Add(RoleModel model)
    {
        try
        {
            Role role = new Role()
            {
                Name = model.RoleName,
                PermissionList = ChangeModelToJSON(model.PermissionGroups),
                IsDefault = false,
                Type = (int)RoleType.DynamicRole,
                IsDeleted = false,
                CreatedBy = _httpContext.User.GetAccountId()
            };

            _unitOfWork.RoleRepository.Add(role);
            _unitOfWork.Save();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Update a role
    /// </summary>
    /// <param name="model"></param>
    public void Update(RoleModel model)
    {
        try
        {
            var role = _unitOfWork.RoleRepository.GetById(model.Id);

            if (role != null)
            {
                role.Name = model.RoleName;
                role.UpdatedOn = DateTime.UtcNow;
                role.UpdatedBy = _httpContext.User.GetAccountId();
                if (model.PermissionGroups != null && model.PermissionGroups.Any())
                {
                    role.PermissionList = ChangeModelToJSON(model.PermissionGroups);
                }

                _unitOfWork.RoleRepository.Update(role);
                _unitOfWork.Save();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="role"></param>
    public void Delete(Role role)
    {
        try
        {
            role.IsDeleted = true;
            role.UpdatedOn = DateTime.UtcNow;
            role.UpdatedBy = _httpContext.User.GetAccountId();
            _unitOfWork.RoleRepository.Update(role);

            _unitOfWork.Save();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Delete roles
    /// </summary>
    /// <param name="roles"></param>
    public void DeleteMultiple(List<Role> roles)
    {
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var updatedBy = _httpContext.User.GetAccountId();
                    foreach (var role in roles)
                    {
                        role.IsDeleted = true;
                        role.UpdatedOn = DateTime.UtcNow;
                        role.UpdatedBy = updatedBy;
                        _unitOfWork.RoleRepository.Update(role);
                    }

                    _unitOfWork.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        });
    }

    public void ChangeDefaultSettingRole(int id)
    {
        var roles = _unitOfWork.AppDbContext.Role.Where(x => !x.IsDeleted).ToList();
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var updatedBy = _httpContext.User.GetAccountId();
                    foreach (var role in roles)
                    {
                        role.IsDefault = role.Id == id;
                        role.UpdatedBy = updatedBy;
                        role.UpdatedOn = DateTime.UtcNow;
                        _unitOfWork.RoleRepository.Update(role);
                    }

                    _unitOfWork.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        });
    }

    /// <summary>
    /// This function changes model data to JSON string form data.
    /// This function is used to store data in DB without name and description.
    /// </summary>
    /// <param name="permissionGroups"> Permission model data </param>
    /// <returns> permission data in JSON string form </returns>
    private string ChangeModelToJSON(List<PermissionGroupModel> permissionGroups)
    {
        if (permissionGroups == null || !permissionGroups.Any())
        {
            return null;
        }

        List<PermissionGroupDataModel> permissionGroupDatas = new List<PermissionGroupDataModel>();

        permissionGroupDatas = permissionGroups.Select(
            m =>
            {
                var permissionGroupData = new PermissionGroupDataModel
                {
                    Title = m.Title,
                    Permissions = m.Permissions.Select(
                        c =>
                        {
                            var permissionData = new PermissionDataModel
                            {
                                Title = c.Title,
                                IsEnabled = c.IsEnabled
                            };

                            return permissionData;
                        }).ToList()
                };

                return permissionGroupData;
            }).ToList();

        var permissionList = JsonConvert.SerializeObject(permissionGroupDatas, Formatting.None);

        return permissionList;
    }

    public Dictionary<string, Dictionary<string, bool>> GetPermissionsByRoleId(int roleId)
    {
        Dictionary<string, Dictionary<string, bool>> result = new Dictionary<string, Dictionary<string, bool>>();
        var permissions = _unitOfWork.RoleRepository.GetById(roleId);
        if (permissions != null)
        {
            var modelData = JsonConvert.DeserializeObject<List<PermissionGroupDataModel>>(permissions.PermissionList);

            if (modelData != null)
            {
                foreach (var model in modelData)
                {
                    Dictionary<string, bool> permission = new Dictionary<string, bool>();

                    foreach (var permissionModel in model.Permissions)
                    {
                        permission.Add(permissionModel.Title, permissionModel.IsEnabled);
                    }

                    result.Add(model.Title, permission);
                }
            }

            return result;
        }

        return result;
    }

    public Role GetById(int roleId)
    {
        return _unitOfWork.AppDbContext.Role.FirstOrDefault(x => x.Id == roleId && !x.IsDeleted);
    }

    public List<Role> GetByIds(List<int> roleIds)
    {
        return _unitOfWork.AppDbContext.Role.Where(x => roleIds.Contains(x.Id) && !x.IsDeleted).ToList();
    }

    /// <summary>
    /// Get default permission value(true/false) by id
    /// This function is used to show on web.
    /// </summary>
    /// <param name="accountType"> integer flag to distinguish that account is primary manager or employee or not </param>
    /// <returns></returns>
    public List<PermissionGroupModel> GetDefaultRoleValue(int accountType = (int)RoleType.DynamicRole)
    {
        var accountLanguage = _unitOfWork.AppDbContext.Account
                                  .FirstOrDefault(m => m.Id == _httpContext.User.GetAccountId())?.Language
                              ?? string.Empty;
        var culture = new CultureInfo(accountLanguage);

        var permissionGroups = new List<PermissionGroupModel>();

        foreach (var eachData in PagePermission.GetAllPermssions())
        {
            permissionGroups.Add(_unitOfWork.RoleRepository.GetListModel(eachData, culture, accountType));
        }

        return permissionGroups;
    }

    /// <summary>
    /// This function changes JSON data to list of model data.
    /// This function is used to get data from DB with name and description about permissions.
    /// </summary>
    /// <param name="permissionList"> Permission data in JSON form </param>
    /// <returns> list of model data </returns>
    public List<PermissionGroupModel> ChangeJsonToModel(string permissionList)
    {
        var accountLanguage = _unitOfWork.AppDbContext.Account
            .FirstOrDefault(m => m.Id == _httpContext.User.GetAccountId())?.Language ?? string.Empty;
        //var companyId = _httpContext.User.GetCompanyId();
        //var companyLanguage = _settingService.GetLanguage(companyId);
        var culture = new CultureInfo(accountLanguage);

        List<PermissionGroupModel> permissionGroupModel = new List<PermissionGroupModel>();

        if (string.IsNullOrEmpty(permissionList))
        {
            return permissionGroupModel;
        }

        var jsonObject = JsonConvert.DeserializeObject<List<PermissionGroupDataModel>>(permissionList);

        if (jsonObject != null)
        {
            permissionGroupModel = jsonObject.Select(
                m =>
                {
                    var permissionGroup = new PermissionGroupModel
                    {
                        Title = m.Title,
                        GroupName = PermissionResource.ResourceManager.GetString(m.Title, culture) ?? string.Empty,
                        Permissions = m.Permissions.Select(
                            c =>
                            {
                                var permission = new PermissionModel
                                {
                                    Title = c.Title,
                                    PermissionName = PermissionResource.ResourceManager.GetString(c.Title, culture) ??
                                                     string.Empty,
                                    Description =
                                        PermissionResource.ResourceManager.GetString(Constants.Description + c.Title,
                                            culture) ?? string.Empty,
                                    IsEnabled = c.IsEnabled
                                };

                                return permission;
                            }).ToList()
                    };

                    return permissionGroup;
                }).ToList();
        }

        return permissionGroupModel;
    }

    public bool CheckPermissions(List<PermissionGroupModel> permissions)
    {
        var permissionGroups = permissions.GroupBy(m => m.Title);

        return !permissionGroups.Any(m => m.Count() > 1);
    }

    public bool IsExistedName(string name, int ignoreId)
    {
        var category = _unitOfWork.RoleRepository.GetByName(name);
        return category != null && category.Id != ignoreId;
    }

    public bool CheckPermissionEnabled(string permissionName, int accountId)
    {
        var role = _unitOfWork.RoleRepository.GetByAccountId(accountId);

        if (role == null)
        {
            return false;
        }
        else
        {
            var permission = role.PermissionGroups.SelectMany(m => m.Permissions).FirstOrDefault(m => m.Title == permissionName);

            if (permission == null)
                return false;

            return permission.IsEnabled;
        }
    }
}