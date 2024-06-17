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
using ERPSystem.DataModel.FriendUser;
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

public interface IFriendUserService
{
    FriendUser? GetById(int friendUserId);
   
    Dictionary<string, object> GetInit();
    List<FriendUserListModel> GetPaginated(int userId, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered);
    int AddFriendUser(FriendUserAddModel model);
    bool UpdateFriendUser(FriendUserEditModel model);
    bool DeleteMultiFriendUsers(List<int> ids);
   
}

public class FriendUserService : IFriendUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailService _mailService;


    public FriendUserService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IMapper mapper, IAccountService accountService, IMailTemplateService mailTemplateService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<FriendUserService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _mapper = mapper;
        _accountService = accountService;
        _mailTemplateService = mailTemplateService;
        _mailService = mailService;

    }

    public FriendUser? GetById(int friendUserId)
    {
        var friendUser = _unitOfWork.FriendUserRepository.Gets().FirstOrDefault(m => m.Id==friendUserId);
        return friendUser;
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
        // friendUser status
        var status = EnumHelper.ToEnumList<Status>();
        result.Add("friendUserStatus", status.OrderBy(m => m.Name));

        return result;
    }

    public List<FriendUserListModel> GetPaginated(int userId, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.FriendUserRepository.Gets();
        totalRecords = data.Count();
        
        if(userId != 0)
        {
            data = data.Where(m => m.UserId1 == userId || m.UserId2 == userId);
        }

        

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return data.AsEnumerable<FriendUser>().Select(_mapper.Map<FriendUserListModel>).ToList();
    }

    public int AddFriendUser(FriendUserAddModel model)
    {
        int friendUserId = 0;
        
        // Add friendUser
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    FriendUser friendUser = _mapper.Map<FriendUser>(model);
               
                    _unitOfWork.FriendUserRepository.Add(friendUser);
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
        
        
        return friendUserId;
    }

    public bool UpdateFriendUser(FriendUserEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit friendUser information
                    var friendUser = _unitOfWork.FriendUserRepository.GetById(model.Id);
                    if (friendUser == null)
                        throw new Exception($"Can not get friendUser by id = {model.Id}");

                    _mapper.Map(model, friendUser);
             
                    _unitOfWork.FriendUserRepository.Update(friendUser);
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

    public bool DeleteMultiFriendUsers(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var friendUsers = _unitOfWork.FriendUserRepository.Gets().Where(m => ids.Contains(m.Id)).ToList();
                    foreach (var friendUser in friendUsers)
                    {
                  
                        // delete friendUser
                        _unitOfWork.FriendUserRepository.Delete(m => m.Id == friendUser.Id);
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