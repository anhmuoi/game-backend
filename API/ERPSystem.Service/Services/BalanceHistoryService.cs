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
using ERPSystem.DataModel.BalanceHistory;
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

public interface IBalanceHistoryService
{
    BalanceHistory? GetById(int balanceHistoryId);
   
    Dictionary<string, object> GetInit();
    List<BalanceHistoryListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered);
    int AddBalanceHistory(BalanceHistoryAddModel model);
    bool UpdateBalanceHistory(BalanceHistoryEditModel model);
    bool DeleteMultiBalanceHistorys(List<int> ids);
   
}

public class BalanceHistoryService : IBalanceHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailService _mailService;


    public BalanceHistoryService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IMapper mapper, IAccountService accountService, IMailTemplateService mailTemplateService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<BalanceHistoryService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _mapper = mapper;
        _accountService = accountService;
        _mailTemplateService = mailTemplateService;
        _mailService = mailService;

    }

    public BalanceHistory? GetById(int balanceHistoryId)
    {
        var balanceHistory = _unitOfWork.BalanceHistoryRepository.Gets().FirstOrDefault(m => m.Id==balanceHistoryId);
        return balanceHistory;
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
        // balanceHistory status
        var status = EnumHelper.ToEnumList<Status>();
        result.Add("balanceHistoryStatus", status.OrderBy(m => m.Name));

        return result;
    }

    public List<BalanceHistoryListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.BalanceHistoryRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(m => m.Name.ToLower().Contains(search.ToLower()));
        }
 

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return data.AsEnumerable<BalanceHistory>().Select(_mapper.Map<BalanceHistoryListModel>).ToList();
    }

    public int AddBalanceHistory(BalanceHistoryAddModel model)
    {
        int balanceHistoryId = 0;
        
        // Add balanceHistory
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    BalanceHistory balanceHistory = _mapper.Map<BalanceHistory>(model);
               
                    _unitOfWork.BalanceHistoryRepository.Add(balanceHistory);
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
        
        
        return balanceHistoryId;
    }

    public bool UpdateBalanceHistory(BalanceHistoryEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit balanceHistory information
                    var balanceHistory = _unitOfWork.BalanceHistoryRepository.GetById(model.Id);
                    if (balanceHistory == null)
                        throw new Exception($"Can not get balanceHistory by id = {model.Id}");

                    _mapper.Map(model, balanceHistory);
             
                    _unitOfWork.BalanceHistoryRepository.Update(balanceHistory);
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

    public bool DeleteMultiBalanceHistorys(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var balanceHistorys = _unitOfWork.BalanceHistoryRepository.Gets().Where(m => ids.Contains(m.Id)).ToList();
                    foreach (var balanceHistory in balanceHistorys)
                    {
                  
                        // delete balanceHistory
                        _unitOfWork.BalanceHistoryRepository.Delete(m => m.Id == balanceHistory.Id);
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