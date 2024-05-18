using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.MeetingLog;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace ERPSystem.Service.Services;

public interface IMeetingLogService
{
    List<DashboardDataModel> GetWorkLogDashboards(DateTime date);
    List<MeetingLogResponseModel> GetPaginated(string search, List<int> meetingRoomIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    bool Add(MeetingLogModel model);
    bool Update(MeetingLogModel model);
    bool Delete(List<int> ids);
    MeetingLogResponseModel? GetById(int id);
    bool CheckCreatedBy(List<int> ids, int accountId);
}
public class MeetingLogService : IMeetingLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HttpContext _httpContext;
    private readonly IMapper _mapper;
    
    public MeetingLogService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<DailyReportService>();
        _mapper = mapper;
        _httpContext = contextAccessor.HttpContext;
    }

    public List<DashboardDataModel> GetWorkLogDashboards(DateTime date)
    {
        return _unitOfWork.MeetingLogRepository
            .Gets(x => 
                (x.StartDate.Value.Date <= date.Date && x.EndDate.Value.Date >= date.Date) ||
                (x.StartDate.Value.Date <= date.AddDays(1).Date && x.EndDate.Value.Date >= date.AddDays(1).Date) ||
                (x.StartDate.Value.Date <= date.AddDays(2).Date && x.EndDate.Value.Date >= date.AddDays(2).Date))
            .AsEnumerable()
            .Select(_mapper.Map<DashboardDataModel>)
            .OrderBy(x => x.StartDate)
            .ToList();
    }
    public List<MeetingLogResponseModel> GetPaginated(string search, List<int> meetingRoomIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        List<MeetingLogResponseModel> result = new List<MeetingLogResponseModel>();
        var currentUser = _httpContext.User.GetAccountId();
        var data = _unitOfWork.MeetingLogRepository.GetAll().AsQueryable();
        
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(x => x.Title.ToLower().Contains(search.ToLower()));
        }
        if (meetingRoomIds is { Count: > 0 })
        {
            data = data.Where(m => meetingRoomIds.Contains(m.MeetingRoomId.Value));
        }
    

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        foreach (var item in data)
        {
            var meetingLog = _mapper.Map<MeetingLogResponseModel>(item);
            meetingLog.IsAction = item.CreatedBy == currentUser;
            result.Add(meetingLog);
        }
        return result;
    }

    public bool Add(MeetingLogModel model)
    {
        try
        {
            var meetingLog = _mapper.Map<MeetingLog>(model);
            
            _unitOfWork.MeetingLogRepository.Add(meetingLog);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(MeetingLogModel model)
    {
        try
        {
            var meetingLog = _unitOfWork.MeetingLogRepository.GetById(model.Id);
            if (meetingLog != null)
            {
                _mapper.Map(model, meetingLog);
            
                _unitOfWork.MeetingLogRepository.Update(meetingLog);
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
            var meetingLogs = _unitOfWork.MeetingLogRepository.GetByIds(ids);
            _unitOfWork.MeetingLogRepository.DeleteRange(meetingLogs);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public MeetingLogResponseModel? GetById(int id)
    {
        try
        {
            var worklog = _unitOfWork.MeetingLogRepository.GetById(id);
            if (worklog != null)
            {
                var result = _mapper.Map<MeetingLogResponseModel>(worklog);
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
        var listMeetingLog = _unitOfWork.MeetingLogRepository.GetByIds(ids);
        if (listMeetingLog.Any())
        {
            return listMeetingLog.All(x => x.CreatedBy == accountId);
        }

        return false;
    }
}