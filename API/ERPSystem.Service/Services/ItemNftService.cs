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
using ERPSystem.DataModel.ItemNft;
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

public interface IItemNftService
{
    ItemNft? GetById(int itemNftId);
   
    Dictionary<string, object> GetInit();
    List<ItemNftListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered);
    int AddItemNft(ItemNftAddModel model);
    bool UpdateItemNft(ItemNftEditModel model);
    bool DeleteMultiItemNfts(List<int> ids);
   
}

public class ItemNftService : IItemNftService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailService _mailService;


    public ItemNftService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IMapper mapper, IAccountService accountService, IMailTemplateService mailTemplateService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<ItemNftService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _mapper = mapper;
        _accountService = accountService;
        _mailTemplateService = mailTemplateService;
        _mailService = mailService;

    }

    public ItemNft? GetById(int itemNftId)
    {
        var itemNft = _unitOfWork.ItemNftRepository.Gets().FirstOrDefault(m => m.Id==itemNftId);
        return itemNft;
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
        // itemNft status
        var status = EnumHelper.ToEnumList<Status>();
        result.Add("itemNftStatus", status.OrderBy(m => m.Name));

        return result;
    }

    public List<ItemNftListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.ItemNftRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(m => m.Name.ToLower().Contains(search.ToLower()));
        }
 

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return data.AsEnumerable<ItemNft>().Select(_mapper.Map<ItemNftListModel>).ToList();
    }

    public int AddItemNft(ItemNftAddModel model)
    {
        int itemNftId = 0;
        
        // Add itemNft
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    ItemNft itemNft = _mapper.Map<ItemNft>(model);
               
                    _unitOfWork.ItemNftRepository.Add(itemNft);
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
        
        
        return itemNftId;
    }

    public bool UpdateItemNft(ItemNftEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit itemNft information
                    var itemNft = _unitOfWork.ItemNftRepository.GetById(model.Id);
                    if (itemNft == null)
                        throw new Exception($"Can not get itemNft by id = {model.Id}");

                    _mapper.Map(model, itemNft);
             
                    _unitOfWork.ItemNftRepository.Update(itemNft);
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

    public bool DeleteMultiItemNfts(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var itemNfts = _unitOfWork.ItemNftRepository.Gets().Where(m => ids.Contains(m.Id)).ToList();
                    foreach (var itemNft in itemNfts)
                    {
                  
                        // delete itemNft
                        _unitOfWork.ItemNftRepository.Delete(m => m.Id == itemNft.Id);
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