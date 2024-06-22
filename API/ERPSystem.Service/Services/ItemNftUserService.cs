using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Login;
using ERPSystem.DataModel.ItemNftUser;
using ERPSystem.Repository;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AutoMapper;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json.Linq;
using MimeKit;

namespace ERPSystem.Service.Services;

public interface IItemNftUserService
{
    ItemNftUser? GetById(int itemNftUserId);
   
    Dictionary<string, object> GetInit();
    List<ItemNftUserListModel> GetPaginated(string search, List<int> status, int userId, bool getAll, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered);
    int AddItemNftUser(ItemNftUserAddModel model);
    bool AssignItemNftForUser(List<int> idItemNftId, int userId);
    bool UpdateItemNftUser(ItemNftUserEditModel model);
    bool UseItem(List<int> idItemList);
    bool DeleteMultiItemNftUsers(List<int> ids);
   
}

public class ItemNftUserService : IItemNftUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailService _mailService;


    public ItemNftUserService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IMapper mapper, IAccountService accountService, IMailTemplateService mailTemplateService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<ItemNftUserService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _mapper = mapper;
        _accountService = accountService;
        _mailTemplateService = mailTemplateService;
        _mailService = mailService;

    }

    public ItemNftUser? GetById(int itemNftUserId)
    {
        var itemNftUser = _unitOfWork.ItemNftUserRepository.Gets().FirstOrDefault(m => m.Id==itemNftUserId);
        return itemNftUser;
    }


    
    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        // account type
        var roleList = _unitOfWork.RoleRepository.Gets(x => !x.IsDeleted);
        List<EnumModelRole> type = new List<EnumModelRole>();
        foreach(var role in roleList)
        {
            type.Add(new EnumModelRole { Id = role.Id, Name = role.Name, IsDefault = role.IsDefault });
        }
        result.Add("roles", type);
        // department
        var departments = _unitOfWork.DepartmentRepository.Gets();
        List<EnumModel> departmentList = new List<EnumModel>();
        foreach(var depart in departments)
        {
            departmentList.Add(new EnumModel { Id = depart.Id, Name = depart.Name });
        }
        result.Add("departments", departments);
        // itemNftUser status
        var status = EnumHelper.ToEnumList<Status>();
        result.Add("itemNftUserStatus", status.OrderBy(m => m.Name));

        return result;
    }

    public List<ItemNftUserListModel> GetPaginated(string search, List<int> status, int userId, bool getAll, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.ItemNftUserRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(m => m.Name.ToLower().Contains(search.ToLower()));
        }
        if (status is { Count: > 0 })
        {
            data = data.Where(m => status.Contains(m.Status));
        }
        if (userId != 0)
        {
            data = data.Where(m => m.UserId == userId);
        }
 

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        if(getAll)
        {

        } else
        {
            data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        return data.AsEnumerable<ItemNftUser>().Select(_mapper.Map<ItemNftUserListModel>).ToList();
    }

    public int AddItemNftUser(ItemNftUserAddModel model)
    {
        int itemNftUserId = 0;
        
        // Add itemNftUser
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    ItemNftUser itemNftUser = _mapper.Map<ItemNftUser>(model);
               
                    _unitOfWork.ItemNftUserRepository.Add(itemNftUser);
                    _unitOfWork.Save();
                
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                }
            }
        });
        
        
        return itemNftUserId;
    }

    public bool UpdateItemNftUser(ItemNftUserEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit itemNftUser information
                    var itemNftUser = _unitOfWork.ItemNftUserRepository.GetById(model.Id);
                    if (itemNftUser == null)
                        throw new Exception($"Can not get itemNftUser by id = {model.Id}");

                    _mapper.Map(model, itemNftUser);
             
                    _unitOfWork.ItemNftUserRepository.Update(itemNftUser);
                    _unitOfWork.Save();


                
                    
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
    public bool UseItem(List<int> idItemList)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach(var itemId in idItemList)
                    {

                        // edit itemNftUser information
                        var itemNftUser = _unitOfWork.ItemNftUserRepository.GetById(itemId);
                        if (itemNftUser == null)
                            throw new Exception($"Can not get itemNftUser by id = {itemId}");
                        itemNftUser.Status = 2;
                
                        _unitOfWork.ItemNftUserRepository.Update(itemNftUser);
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
    public bool AssignItemNftForUser(List<int> idItemNftId, int userId)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                 
                    foreach(var idNft in idItemNftId)
                    {
                        _unitOfWork.ItemNftUserRepository.AddItemNftUser(idNft, userId, 0);
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

    public bool DeleteMultiItemNftUsers(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var itemNftUsers = _unitOfWork.ItemNftUserRepository.Gets().Where(m => ids.Contains(m.Id)).ToList();
                    foreach (var itemNftUser in itemNftUsers)
                    {
                  
                        // delete itemNftUser
                        _unitOfWork.ItemNftUserRepository.Delete(m => m.Id == itemNftUser.Id);
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
    

    
    
   
    
  

   
}