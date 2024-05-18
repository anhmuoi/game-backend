using System.Linq.Dynamic.Core;
using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Account;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERPSystem.Service.Services;

public interface IFolderLogService
{
    Dictionary<string, object> GetInit();
    List<FolderLogListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    List<FolderLogSearch> Search(string search);
    bool Add(FolderLogModel model);
    bool Update(FolderLogModel model);
    bool Delete(List<int> ids);
    FolderLogListModel? GetById(int id);
    List<FolderLogListModel> GetByIds(List<int> ids);
    bool IsExistedNameInLevel(string name, int? parentId, int ignoreId);
}
public class FolderLogService : IFolderLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly HttpContext _httpContext;
    
    public FolderLogService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<FolderLogService>();
        _mapper = mapper;
        _httpContext = contextAccessor.HttpContext;
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // account
        var accounts = _unitOfWork.AppDbContext.Account.Include(x => x.User).Select(x => new AccountListModelInit()
        {
            Id = x.Id,
            Name = x.UserName,
            Avatar = x.User!.Avatar ?? ""
        });
        data.Add("accounts", accounts);
        
        return data;
    }

    public List<FolderLogListModel> GetPaginated(string search, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.AppDbContext.FolderLog
            .Include(x => x.Parent)
            .Include(x => x.WorkLog)
            .Include(x => x.DailyReport)
            .AsQueryable();
        
        // var listFolderLog = data.AsEnumerable().Select(_mapper.Map<FolderLogListModel>).ToList();
        
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(x => x.Name.ToLower().Contains(search.ToLower()) 
                              || x.WorkLog.Any(z => z.Title.ToLower().Contains(search.ToLower()) || z.Content.Contains(search))
                              || x.DailyReport.Any(z => z.Title.ToLower().Contains(search.ToLower()) || z.Content.Contains(search))
                              );
        }

        recordsFiltered = data.Count();
        if (recordsFiltered == 0)
        {
            return new List<FolderLogListModel>();
        }
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        // data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var folderLogs = data.AsEnumerable().Select(_mapper.Map<FolderLogListModel>).ToList();
        var folderLogIds = folderLogs.Select(x => x.Id).ToList();
        // get list work schedule
        var workSchedules = _unitOfWork.WorkScheduleRepository.GetByFolderLogIds(folderLogIds, search).ToList();
        // get list work log
        var workLogs = _unitOfWork.WorkLogRepository.GetByFolderLogIds(folderLogIds, search).ToList();
        // get list daily report
        var dailyReports = _unitOfWork.DailyReportRepository.GetByFolderLogIds(folderLogIds, search).ToList();
        
        foreach (var item in folderLogs)
        {
            var lstChilFolder = GenerateTree(folderLogs, item, workSchedules, null, workLogs, dailyReports);

            item.Children = lstChilFolder;
            item.Schedule = workSchedules.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                .ToList();
           
            item.WorkLog = workLogs.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                .ToList();
            item.DailyReport = dailyReports.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                .ToList();
        }

        var resultList = folderLogs.Where(x => x.ParentId == 0 || x.ParentId == null).ToList();
        if (totalRecords != recordsFiltered)
        {
            resultList = folderLogs.ToList();
        }

        recordsFiltered = resultList.Count;

        if (pageSize > 0)
        {
            resultList = resultList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        return resultList;
    }

    public List<FolderLogSearch> Search(string search)
    {
        List<FolderLogSearch> result = new List<FolderLogSearch>();

        if (!string.IsNullOrEmpty(search))
        {
            DateTime startConvert = DateTime.Now;
            search = System.Net.WebUtility.HtmlEncode(search);
            foreach (var replacement in Constants.HtmlDecToEntityMap)
            {
                search = search.Replace(replacement.Value, replacement.Key);
            }
            Console.WriteLine($"TIME CONVERT SEARCH TEXT: {DateTime.Now.Subtract(startConvert).TotalSeconds} seconds");
            // folder log
            DateTime startFolder = DateTime.Now;
            var listFolder = _unitOfWork.FolderLogRepository.Gets(x => x.Name.ToLower().Contains(search.ToLower())).AsEnumerable()
                .Select(_mapper.Map<FolderLogSearch>).ToList();
            result.AddRange(listFolder);
        
            Console.WriteLine($"TIME QUERY FOLDERLOG: {DateTime.Now.Subtract(startFolder).TotalSeconds} seconds");
            DateTime startMeeting = DateTime.Now;
            // meeting log
            var meetingLog = _unitOfWork.MeetingLogRepository.Gets(x => x.Title.ToLower().Contains(search.ToLower())
                                                                        || x.Content.Contains(search)).AsEnumerable().Select(_mapper.Map<FolderLogSearch>).ToList();
            result.AddRange(meetingLog);
            Console.WriteLine($"TIME QUERY METTINGLOG: {DateTime.Now.Subtract(startMeeting).TotalSeconds} seconds");
            DateTime startWorkLog = DateTime.Now;
        
            // work log
            var workLog = _unitOfWork.WorkLogRepository.Gets(x => x.Title.ToLower().Contains(search.ToLower())
                                                                  || x.Content.Contains(search)).AsEnumerable().Select(_mapper.Map<FolderLogSearch>).ToList();
            result.AddRange(workLog);
            Console.WriteLine($"TIME QUERY WORKLOG: {DateTime.Now.Subtract(startWorkLog).TotalSeconds} seconds");
            Console.WriteLine($"TOTAL TIME: {DateTime.Now.Subtract(startFolder).TotalSeconds} seconds");
        }
        
        return result;
    }

    private List<FolderLogListModel> GenerateTree(List<FolderLogListModel> collection, FolderLogListModel rootItem, 
        List<WorkSchedule> workSchedules, List<MeetingLog> meetings, List<WorkLog> workLogs, List<DailyReport> dailyReports)
    {
        List<FolderLogListModel> lst = new List<FolderLogListModel>();
        var listCollection = collection.Where(m => m.ParentId == rootItem.Id).ToList();
        foreach (FolderLogListModel item in listCollection)
        {
            lst.Add(new FolderLogListModel
            {
                Id = item.Id,
                Name = item.Name,
                Children = GenerateTree(collection.Where(x => x.Id != item.Id).ToList(), item, workSchedules, meetings, workLogs, dailyReports),
                ParentId = rootItem.Id,
                Description = item.Description,
                UpdatedBy = item.UpdatedBy,
                UpdatedOn = item.UpdatedOn,
                Schedule = workSchedules.Where(x => x.FolderLogId == item.Id).AsEnumerable()
                    .Select(_mapper.Map<ChildFolderModel>).ToList(),
                WorkLog = workLogs.Where(x => x.FolderLogId == item.Id).AsEnumerable()
                    .Select(_mapper.Map<ChildFolderModel>).ToList(),
                DailyReport = dailyReports.Where(x => x.FolderLogId == item.Id).AsEnumerable()
                    .Select(_mapper.Map<ChildFolderModel>).ToList(),
            });
        }
        return lst;
    }

    public bool Add(FolderLogModel model)
    {
        try
        {
            var folderLog = _mapper.Map<FolderLog>(model);
            
            _unitOfWork.FolderLogRepository.Add(folderLog);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(FolderLogModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var folderLog = _unitOfWork.FolderLogRepository.GetById(model.Id);
                    if (folderLog != null)
                    {
                        var currentParent = folderLog.ParentId;

                        _mapper.Map(model, folderLog);
            
                        _unitOfWork.FolderLogRepository.Update(folderLog);
                        _unitOfWork.Save();

                        if (model.ParentId != null && model.ParentId != 0)
                        {
                            var folderLogModel = _unitOfWork.FolderLogRepository.GetById(model.ParentId ?? 0);
                            if (folderLogModel?.ParentId != null && folderLogModel.Id != currentParent)
                            {
                                folderLogModel.ParentId = currentParent;
                                _unitOfWork.FolderLogRepository.Update(folderLogModel);
                                _unitOfWork.Save();
                            }
                        }
                        transaction.Commit();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    result = false;
                }
            }
        });
        return result;
    }

    public bool Delete(List<int> ids)
    {
        try
        {
            var folderLogs = _unitOfWork.FolderLogRepository.GetByIds(ids);
            _unitOfWork.FolderLogRepository.DeleteRange(folderLogs);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public FolderLogListModel? GetById(int id)
    {
        try
        {
            var folderLog = _unitOfWork.FolderLogRepository.GetById(id);
            if (folderLog != null)
            {
                var result = _mapper.Map<FolderLogListModel>(folderLog);

                result.Schedule = _unitOfWork.WorkScheduleRepository.GetByFolderLogId(folderLog.Id).AsEnumerable()
                     .Select(_mapper.Map<ChildFolderModel>).ToList();
                
                result.WorkLog = _unitOfWork.WorkLogRepository.GetByFolderLogId(folderLog.Id).AsEnumerable()
                    .Select(_mapper.Map<ChildFolderModel>).ToList();
                result.DailyReport = _unitOfWork.DailyReportRepository.GetByFolderLogId(folderLog.Id).AsEnumerable()
                    .Select(_mapper.Map<ChildFolderModel>).ToList();
                
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

   public List<FolderLogListModel> GetByIds(List<int> ids)
    {
        try
        {
            var folderLogs = _unitOfWork.FolderLogRepository.GetByIdsAndChild(ids)
                .AsEnumerable().Select(_mapper.Map<FolderLogListModel>).ToList();
            
            var folderLogIds = folderLogs.Select(x => x.Id).ToList();
            // get list work schedule
            var workSchedules = _unitOfWork.WorkScheduleRepository.GetByFolderLogIds(folderLogIds).ToList();
            // get list meeting
            // get list work log
            var workLogs = _unitOfWork.WorkLogRepository.GetByFolderLogIds(folderLogIds).ToList();
            // get list daily report
            var dailyReports = _unitOfWork.DailyReportRepository.GetByFolderLogIds(folderLogIds).ToList();
        
            foreach (var item in folderLogs)
            {
                var lstChilFolder = GenerateTree(folderLogs, item, workSchedules, null, workLogs, dailyReports);
                
                item.Children = lstChilFolder;
                item.Schedule = workSchedules.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                    .ToList();
              
                item.WorkLog = workLogs.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                    .ToList();
                item.DailyReport = dailyReports.Where(x => x.FolderLogId == item.Id).AsEnumerable().Select(_mapper.Map<ChildFolderModel>)
                    .ToList();
            }

            return folderLogs.Where(x => ids.Contains(x.Id)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return new List<FolderLogListModel>();
        }
    }

    public bool IsExistedNameInLevel(string name, int? parentId, int ignoreId)
    {
        var folder = _unitOfWork.FolderLogRepository.GetByName(name).FirstOrDefault(m => m.ParentId == parentId);
        return folder != null && folder.Id != ignoreId;
    }
}