using System.Data.Entity;
using System.Linq.Dynamic.Core;
using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Account;
using ERPSystem.DataModel.DailyReport;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERPSystem.Service.Services;

public interface IDailyReportService
{
    Dictionary<string, object> GetInit();
    List<DailyReportResponseModel> GetPaginated(string search, List<int> userIds, List<int> folderLogs, List<int> reporters, List<int> departmentIds, DateTime start, DateTime end, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    bool Add(DailyReportModel model);
    bool Update(DailyReportModel model);
    bool Delete(List<int> ids);
    DailyReportResponseModel? GetById(int id);
    DailyReportResponseModel? GetByUserIdAndDate(int userId, string date);
    bool CheckCreatedBy(List<int> ids, int accountId);
}

public class DailyReportService : IDailyReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HttpContext _httpContext;
    private readonly IMapper _mapper;
    
    public DailyReportService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<DailyReportService>();
        _mapper = mapper;
        _httpContext = contextAccessor.HttpContext;
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // reporter
        var reporter = _unitOfWork.AccountRepository.GetAll().Select(x => new ReporterModel()
        {
            Id = x.Id,
            Name = x.UserName,
        });
        data.Add("reporter", reporter);
        // department
        var department = _unitOfWork.DepartmentRepository.GetAll().Select(x => new ReporterModel()
        {
            Id = x.Id,
            Name = x.Name,
        });
        data.Add("departments", department);

        // folder log
        var folderLog = _unitOfWork.FolderLogRepository.GetAll().Select(x => new ReporterModel()
        {
            Id = x.Id,
            Name = x.Name,
        });
        data.Add("folderLog", folderLog);
        
        return data;
    }

    public List<DailyReportResponseModel> GetPaginated(string search, List<int> userIds, List<int> folderLogs, List<int> reporters, List<int> departmentIds, DateTime start, DateTime end, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        List<DailyReportResponseModel> result = new List<DailyReportResponseModel>();
        var currentUser = _httpContext.User.GetAccountId();
        var data = _unitOfWork.DailyReportRepository.GetAll().AsQueryable().Include(m => m.User);
        
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(x => x.Title.ToLower().Contains(search.ToLower()));
        }
        if (userIds is { Count: > 0 })
        {
            data = data.Where(m => userIds.Contains(m.UserId));
        }
        if (reporters is { Count: > 0 })
        {
            data = data.Where(m => m.ReporterId.HasValue && reporters.Contains(m.ReporterId.Value));
        }
        if (folderLogs is { Count: > 0 })
        {
            data = data.Where(m => folderLogs.Contains(m.FolderLogId.Value));
        }
        
        data = (start != DateTime.MinValue && start <= end) ? data.Where(o => o.Date >= start && o.Date <= end) : data;

        recordsFiltered = data.Count();
        if(sortColumn.ToLower() != "departmentname")
        {

            data = data.OrderBy($"{sortColumn} {sortDirection}");
            data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
        
        foreach (var item in data)
        {
            var dailyReport = _mapper.Map<DailyReportResponseModel>(item);
            dailyReport.IsAction = item.CreatedBy == currentUser;

            var user = _unitOfWork.UserRepository.GetById(item.UserId);
            var department = _unitOfWork.DepartmentRepository.GetById(user.DepartmentId ?? 0);
            if(department != null)
            {
                dailyReport.DepartmentName = department.Name;
            }
            if (departmentIds is { Count: > 0 } && user.DepartmentId.HasValue)
            {
                if (departmentIds.Contains(user.DepartmentId.Value)){

                    result.Add(dailyReport);
                }
            } else {

                result.Add(dailyReport);
            }
            

        }
        // sort by department
        if (sortColumn.ToLower() == "departmentname" && sortDirection.ToLower() == "asc")
        {
            result = result.OrderBy(r => r.DepartmentName).ToList();
            // Trả về dữ liệu theo kích thước trang cho phần sắp xếp theo nhóm
            return result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        else if (sortColumn.ToLower() == "departmentname" && sortDirection.ToLower() == "desc")
        {
            result = result.OrderByDescending(r => r.DepartmentName).ToList();
            return result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        return result;
    }

    public bool Add(DailyReportModel model)
    {
        try
        {
            var dailyReport = _mapper.Map<DailyReport>(model);
            
            _unitOfWork.DailyReportRepository.Add(dailyReport);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(DailyReportModel model)
    {
        try
        {
            var dailyReport = _unitOfWork.DailyReportRepository.GetById(model.Id);
            if (dailyReport != null)
            {
                _mapper.Map(model, dailyReport);
            
                _unitOfWork.DailyReportRepository.Update(dailyReport);
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
            var dailyReports = _unitOfWork.DailyReportRepository.GetByIds(ids);
            _unitOfWork.DailyReportRepository.DeleteRange(dailyReports);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public DailyReportResponseModel? GetById(int id)
    {
        try
        {
            var dailyReport = _unitOfWork.DailyReportRepository.GetById(id);
            if (dailyReport != null)
            {
                var result = _mapper.Map<DailyReportResponseModel>(dailyReport);
                result.IsAction = dailyReport.CreatedBy == _httpContext.User.GetAccountId();
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
    public DailyReportResponseModel? GetByUserIdAndDate(int userId, string date)
    {
        try
        {
            var dateTemp = date.ConvertDefaultStringToDateTime();
            var dailyReport = _unitOfWork.DailyReportRepository.GetAll().Where(m=> m.UserId == userId && m.Date.Date == dateTemp.Date).FirstOrDefault();
            
            if (dailyReport != null)
            {
                var result = _mapper.Map<DailyReportResponseModel>(dailyReport);
                result.IsAction = dailyReport.CreatedBy == _httpContext.User.GetAccountId();
                return result;
            }
            else 
            {
                var accountId = _httpContext.User.GetAccountId();
                var newDailyReport = new DailyReport();
                newDailyReport.UserId = userId;
                newDailyReport.Date = dateTemp;
                newDailyReport.CreatedOn = dateTemp;
                newDailyReport.UpdatedOn = dateTemp;
                newDailyReport.CreatedBy = accountId;
                newDailyReport.UpdatedBy = accountId;
                newDailyReport.Title = "";
                newDailyReport.Content = "";

                _unitOfWork.DailyReportRepository.Add(newDailyReport);
                _unitOfWork.Save();
                var result = _mapper.Map<DailyReportResponseModel>(newDailyReport);
                result.IsAction = newDailyReport.CreatedBy == accountId;
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
        var listDailyReports = _unitOfWork.DailyReportRepository.GetByIds(ids);
        if (listDailyReports.Any())
        {
            return listDailyReports.All(x => x.CreatedBy == accountId);
        }

        return false;
    }
}