using System.Linq.Dynamic.Core;
using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.WorkLog;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERPSystem.Service.Services;

public interface IWorkLogService
{
    List<WorkLogResponseModel> GetPaginated(string search, List<int> userIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    bool Add(WorkLogModel model);
    bool Update(WorkLogModel model);
    bool Delete(List<int> ids);
    WorkLogResponseModel? GetById(int id);
    bool CheckCreatedBy(List<int> ids, int accountId);
}
public class WorkLogService : IWorkLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly HttpContext _httpContext;
    
    public WorkLogService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<WorkLogService>();
        _mapper = mapper;
        _httpContext = contextAccessor.HttpContext;
    }

    public List<WorkLogResponseModel> GetPaginated(string search, List<int> userIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        List<WorkLogResponseModel> result = new List<WorkLogResponseModel>();
        var currentUser = _httpContext.User.GetAccountId();
        var data = _unitOfWork.WorkLogRepository.GetAll().AsQueryable();
        
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(x => x.Title.ToLower().Contains(search.ToLower()));
        }
        if (userIds is { Count: > 0 })
        {
            data = data.Where(m => userIds.Contains(m.UserId));
        }
        if (folderLogs is { Count: > 0 })
        {
            data = data.Where(m => folderLogs.Contains(m.FolderLogId));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        foreach (var item in data)
        {
            var workLog = _mapper.Map<WorkLogResponseModel>(item);
            workLog.IsAction = item.CreatedBy == currentUser;
            result.Add(workLog);
        }
        return result;
    }

    public bool Add(WorkLogModel model)
    {
        try
        {
            var workLog = _mapper.Map<WorkLog>(model);
            
            _unitOfWork.WorkLogRepository.Add(workLog);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(WorkLogModel model)
    {
        try
        {
            var workLog = _unitOfWork.WorkLogRepository.GetById(model.Id);
            if (workLog != null)
            {
                _mapper.Map(model, workLog);
            
                _unitOfWork.WorkLogRepository.Update(workLog);
                _unitOfWork.Save();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
        return false;
    }

    public bool Delete(List<int> ids)
    {
        try
        {
            var workLogs = _unitOfWork.WorkLogRepository.GetByIds(ids);
            _unitOfWork.WorkLogRepository.DeleteRange(workLogs);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public WorkLogResponseModel? GetById(int id)
    {
        try
        {
            var worklog = _unitOfWork.WorkLogRepository.GetById(id);
            if (worklog != null)
            {
                var result = _mapper.Map<WorkLogResponseModel>(worklog);
                result.IsAction = worklog.CreatedBy == _httpContext.User.GetAccountId();
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return null;
        }
    }

    public bool CheckCreatedBy(List<int> ids, int accountId)
    {
        var listWorkLog = _unitOfWork.WorkLogRepository.GetByIds(ids);
        if (listWorkLog.Any())
        {
            return listWorkLog.All(x => x.CreatedBy == accountId);
        }

        return false;
    }
}