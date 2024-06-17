using AutoMapper;
using ERPSystem.Common;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Department;
using ERPSystem.Repository;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using ERPSystem.DataModel.User;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Service.Services;

public interface IDepartmentService
{
    bool IsExistedNumber(string number, int ignoreId);
    bool IsExistedName(string name, int ignoreId);
    List<DepartmentListModel> Gets(DepartmentFilterModel filter, out int totalRecords, out int recordsFiltered);
    Department? GetById(int id);
    bool Add(DepartmentAddModel model);
    bool Edit(DepartmentEditModel model);
    bool RequestJoinGroup(int id, int userId);
    bool ConfirmJoinGroup(int id, int userId, bool confirm);
    bool Delete(List<int> ids);
    bool CheckDepentdance(List<int> ids);
    List<DepartmentItemModel> GetDepartmentTree();
    bool AssignUsersToDepartment(int id, List<int> userIds);
    List<UserListModel> GetUsersByDepartmentId(UserFilterModel filter, out int totalRecords, out int recordsFiltered);
    List<UserListModel> GetUsersWithoutDepartmentId(UserFilterModel filter, out int totalRecords, out int recordsFiltered);
}

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public DepartmentService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<DepartmentService>();
    }

    public bool IsExistedNumber(string number, int ignoreId)
    {
        var department = _unitOfWork.DepartmentRepository.GetByNumber(number);
        return department != null && department.Id != ignoreId;
    }
    
    public bool IsExistedName(string name, int ignoreId)
    {
        var department = _unitOfWork.DepartmentRepository.GetByName(name);
        return department != null && department.Id != ignoreId;
    }
    
    public List<DepartmentListModel> Gets(DepartmentFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.DepartmentRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if (!string.IsNullOrEmpty(filter.Number))
        {
            data = data.Where(m => m.Number.ToLower().Contains(filter.Number));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        return data.AsEnumerable().Select(_mapper.Map<DepartmentListModel>).ToList();
    }

    public Department? GetById(int id)
    {
        return _unitOfWork.DepartmentRepository.GetById(id);
    }
    
    public bool Add(DepartmentAddModel model)
    {
        bool result = false;
        try
        {
            var account = _unitOfWork.AccountRepository.Gets().FirstOrDefault(m => m.Id == model.DepartmentManagerId);
            var department = _mapper.Map<Department>(model);
            department.DepartmentManagerId = account?.Id;
            _unitOfWork.DepartmentRepository.Add(department);
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }
    
    public bool Edit(DepartmentEditModel model)
    {
        bool result = false;
        try
        {
            var department = _unitOfWork.DepartmentRepository.GetById(model.Id);
            if (department == null)
                throw new Exception($"Can not get department by id = {model.Id}");
            
            var account = _unitOfWork.AccountRepository.Gets().FirstOrDefault(m => m.Id == model.DepartmentManagerId);
            
            _mapper.Map(model, department);

            department.DepartmentManagerId = account?.Id;
            _unitOfWork.DepartmentRepository.Update(department);
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }
    
    public bool Delete(List<int> ids)
    {
        bool result = false;
        try
        {
            _unitOfWork.DepartmentRepository.Delete(m => ids.Contains(m.Id));
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }

    public bool CheckDepentdance(List<int> ids)
    {
        var departmentmanager = _unitOfWork.DepartmentRepository.Gets(x => ids.Contains(x.Id) 
                                                                           && x.DepartmentManagerId != null);
        if (!departmentmanager.Any())
        {
            return false;
        }
        
        var user = _unitOfWork.UserRepository.Gets(x => ids.Contains(x.DepartmentId ?? 0));
        if (!user.Any())
        {
            return false;
        }

        return true;
    }

    public List<DepartmentItemModel> GetDepartmentTree()
    {
        List<DepartmentItemModel> data = new List<DepartmentItemModel>();
        var departments = _unitOfWork.DepartmentRepository.Gets().AsEnumerable()
            .Select(_mapper.Map<DepartmentItemModel>).ToList();
        
        foreach (var item in departments)
        {
            item.Children = GenerateTree(departments, item);
            data.Add(item);
        }
        
        return data.Where(m => m.ParentId == null).OrderBy(m => m.Name).ToList();
    }

    private List<DepartmentItemModel> GenerateTree(List<DepartmentItemModel> data, DepartmentItemModel rootItem)
    {
        List<DepartmentItemModel> result = new List<DepartmentItemModel>();
        var children = data.Where(m => m.ParentId == rootItem.Id);
        foreach (var item in children)
        {
            item.Children = GenerateTree(data, item);
            result.Add(item);
        }

        return result;
    }

    public bool AssignUsersToDepartment(int id, List<int> userIds)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var users = _unitOfWork.UserRepository.Gets(m => userIds.Contains(m.Id));
                    foreach (var user in users)
                    {
                        user.DepartmentId = id;
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.Save();
                    }
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }
    public bool RequestJoinGroup(int id, int userId)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var department = _unitOfWork.DepartmentRepository.GetById(id);
                    var userRequest = _unitOfWork.UserRepository.GetById(userId);

                    var account = _unitOfWork.AccountRepository.GetById(department.DepartmentManagerId.Value);
                    var user = _userService.GetUserByAccountId(account.Id);
                   _userService.SendJoinGroupEmail(account, user.Email, userRequest, department);
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }
    public bool ConfirmJoinGroup(int id, int userId, bool confirm)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    if(confirm == true)
                    {
                        var user = _unitOfWork.UserRepository.GetById(userId);
                        user.DepartmentId = id;
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.Save();
                    }


                    var department = _unitOfWork.DepartmentRepository.GetById(id);
                    var userRequest = _unitOfWork.UserRepository.GetById(userId);

                    var account = _unitOfWork.AccountRepository.GetByUserId(userId);

            

                   _userService.SendConfirmJoinGroupEmail(account, userRequest.Email, department, confirm);
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        return result;
    }

    public List<UserListModel> GetUsersByDepartmentId(UserFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var status = new List<int>() { (int)Common.Infrastructure.Status.Valid };
        var data = _unitOfWork.UserRepository.Gets().Where(m=> status.Contains(m.Status))
            .Include(m => m.Account).AsQueryable();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if (filter.DepartmentIds is { Count: > 0 })
        {
            data = data.Where(m => m.DepartmentId.HasValue && filter.DepartmentIds.Contains(m.DepartmentId.Value));
        }
        
        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        
        return data.AsEnumerable().Select(_mapper.Map<UserListModel>).ToList();
    }
    
    public List<UserListModel> GetUsersWithoutDepartmentId(UserFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var status = new List<int>() { (int)Common.Infrastructure.Status.Valid };
        var data = _unitOfWork.UserRepository.Gets().Where(m=> status.Contains(m.Status))
            .Include(m => m.Account).AsQueryable();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if (filter.DepartmentIds is { Count: > 0 })
        {
            data = data.Where(m => !m.DepartmentId.HasValue || !filter.DepartmentIds.Contains(m.DepartmentId.Value));
        }
        
        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        
        return data.AsEnumerable().Select(_mapper.Map<UserListModel>).ToList();
    }
}