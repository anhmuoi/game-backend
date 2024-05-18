using System.Linq.Dynamic.Core;
using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Account;
using ERPSystem.DataModel.Category;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.WorkSchedule;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERPSystem.Service.Services;

public interface IWorkScheduleService
{
    Dictionary<string, object> GetInit();
    List<WorkScheduleListModel> GetPaginated(string search, string accounts, DateTime date, string categoryTypes, List<int> types, List<int> categories,List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    List<DashboardDataModel> GetScheduleDashboards(DateTime date);
    bool Add(WorkScheduleModel model);
    bool Update(WorkScheduleModel model);
    bool Delete(int id);
    bool DeleteMulti(List<int> ids);
    WorkSchedule GetById(int id);
    bool CheckCreatedBy(List<int> ids, int accountId);
    WorkScheduleModel CheckIsAllDay(WorkSchedule schedule);
}
public class WorkScheduleService : IWorkScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly HttpContext _httpContext;
    public WorkScheduleService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContext = contextAccessor.HttpContext;
        _mapper = mapper;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<WorkScheduleService>();
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // category
        var categories = _unitOfWork.CategoryRepository.GetAll().Select(x => new CategoryListModel()
        {
            Id = x.Id,
            Type = x.Type,
            Name = x.Name,
            Color = x.Color
        }).OrderBy(x => x.Name);
        data.Add("categories", categories);
        
        // type
        var types = EnumHelper.ToEnumList<WorkScheduleType>();
        data.Add("types", types.OrderBy(m => m.Name));
        
        // account created schedule
        var accounts = _unitOfWork.AppDbContext.Account.Include(x => x.User).Select(x => new AccountListModelInit()
        {
            Id = x.Id,
            Email = x.UserName,
            Name = x.User!.Name ?? "",
            Avatar = x.User!.Avatar ?? ""
        }).OrderBy(x => x.Name);
        data.Add("accounts", accounts);
        
        // category type
        var categoryTypes = EnumHelper.ToEnumList<CategoryType>();
        data.Add("categoryTypes", categoryTypes);
        
        return data;
    }

    public List<WorkScheduleListModel> GetPaginated(string search, string accounts, DateTime date, string categoryTypes, List<int> types, List<int> categories, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn, string sortDirection,
        out int totalRecords, out int recordsFiltered)
    {
        try
        {
            var currentUser = _httpContext.User.GetAccountId();
            if (date == DateTime.MinValue)
            {
                date = DateTime.Now;
            }
            DateTime startDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime startOfRange = startDate.AddDays(-7).Date;
            DateTime endOfRange = endDate.AddDays(7).Date;

            var data = _unitOfWork.WorkScheduleRepository
                .Gets(x => (x.Type == (int)WorkScheduleType.Public
                            || (x.Type == (int)WorkScheduleType.Private && x.CreatedBy == currentUser))
                           && (!(x.StartDate <= startOfRange && x.EndDate <= startOfRange) &&
                               !(x.StartDate >= endOfRange && x.EndDate >= endOfRange))
                ).AsEnumerable()
                .Select(_mapper.Map<WorkScheduleListModel>).AsQueryable();

            totalRecords = data.Count();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x => x.Title.ToLower().Contains(search.ToLower()));
            }
            if (types is { Count: > 0 })
            {
                data = data.Where(m => types.Contains(m.Type));
            }
            if (categories is { Count: > 0 })
            {
                data = data.Where(m => categories.Contains(m.CategoryId));
            }
            if (folderLogs is { Count: > 0 })
            {
                data = data.Where(m => folderLogs.Contains(m.FolderLogId));
            }
            if (!string.IsNullOrEmpty(accounts))
            {
                String[] strlist = accounts.Split(",");
                if (strlist.Any())
                {
                    List<int> listAccountIds = strlist.Select(int.Parse).ToList();
                    data = data.Where(x => listAccountIds.Contains(x.CreatedBy));
                }
            }
            var listCategory = data.Select(x => x.CategoryId).ToList();
            if (!string.IsNullOrEmpty(categoryTypes))
            {
                String[] strlist = categoryTypes.Split(",");
                if (strlist.Any())
                {
                    List<int> listCategoryTypes = strlist.Select(int.Parse).ToList();
                    var listCategoryTypeHoliday = _unitOfWork.AppDbContext.Category.Where(x => listCategory.Contains(x.Id)
                        && listCategoryTypes.Contains(x.Type)).Select(x => x.Id);
                    data = data.Where(x => listCategoryTypeHoliday.Contains(x.CategoryId));
                }
            }

            recordsFiltered = data.Count();
            data = data.OrderBy($"{sortColumn} {sortDirection}");
            data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return data.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            totalRecords = 0;
            recordsFiltered = 0;
            return new List<WorkScheduleListModel>();
        }
    }

    public List<DashboardDataModel> GetScheduleDashboards(DateTime date)
    {
        return _unitOfWork.WorkScheduleRepository
            .Gets(x =>
                ((x.StartDate.Date <= date.Date && x.EndDate.Date >= date.Date) ||
                 (x.StartDate.Date <= date.AddDays(1).Date && x.EndDate.Date >= date.AddDays(1).Date) ||
                 (x.StartDate.Date <= date.AddDays(2).Date && x.EndDate.Date >= date.AddDays(2).Date)) && x.Type == (short)WorkScheduleType.Public)
            .AsEnumerable()
            .Select(_mapper.Map<DashboardDataModel>)
            .OrderBy(x => x.StartDate)
            .ToList();
    }

    public bool Add(WorkScheduleModel model)
    {
        try
        {
            var workSchedule = _mapper.Map<WorkSchedule>(model);
            if (model.IsAllDay)
            {
                var timeZone = _httpContext.User.GetTimezone();
                var startDate = model.StartDate.ConvertDefaultStringToDateTime().ConvertDateTimeUTCToSystemTimeZone(timeZone);
                var endDate = model.EndDate.ConvertDefaultStringToDateTime().ConvertDateTimeUTCToSystemTimeZone(timeZone);
                
                workSchedule.StartDate = new DateTime(startDate.Year, 
                    startDate.Month,
                    startDate.Day, 
                    0, 0, 0).ConvertTimeUserToUTC(timeZone); // Set to 12:00 AM

                workSchedule.EndDate = new DateTime(endDate.Year,
                    endDate.Month,
                    endDate.Day,
                    23, 59, 59).ConvertTimeUserToUTC(timeZone); // Set to 11:59 PM
            }
            // workSchedule.FolderLogId = model.FolderLogId == 0 ? null : model.FolderLogId;
            _unitOfWork.WorkScheduleRepository.Add(workSchedule);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(WorkScheduleModel model)
    {
        try
        {
            var workSchedule = _unitOfWork.WorkScheduleRepository.GetById(model.Id);
            if (workSchedule != null)
            {
                _mapper.Map(model, workSchedule);
                if (model.IsAllDay)
                {
                    var timeZone = _httpContext.User.GetTimezone();
                    var startDate = model.StartDate.ConvertDefaultStringToDateTime().ConvertDateTimeUTCToSystemTimeZone(timeZone);
                    var endDate = model.EndDate.ConvertDefaultStringToDateTime().ConvertDateTimeUTCToSystemTimeZone(timeZone);

                    workSchedule.StartDate = new DateTime(startDate.Year, 
                        startDate.Month,
                        startDate.Day, 
                        0, 0, 0).ConvertTimeUserToUTC(timeZone); // Set to 12:00 AM

                    workSchedule.EndDate = new DateTime(endDate.Year,
                        endDate.Month,
                        endDate.Day,
                        23, 59, 59).ConvertTimeUserToUTC(timeZone); // Set to 11:59 PM
                }
            
                _unitOfWork.WorkScheduleRepository.Update(workSchedule);
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

    public bool Delete(int id)
    {
        try
        {
            var workSchedule = _unitOfWork.WorkScheduleRepository.GetById(id);
            if (workSchedule != null)
            {
                _unitOfWork.WorkScheduleRepository.Delete(workSchedule);
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

    public bool DeleteMulti(List<int> ids)
    {
        try
        {
            var workSchedules = _unitOfWork.WorkScheduleRepository.GetByIds(ids);
            _unitOfWork.WorkScheduleRepository.DeleteRange(workSchedules);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public WorkSchedule GetById(int id)
    {
        return _unitOfWork.WorkScheduleRepository.GetById(id);
    }

    public bool CheckCreatedBy(List<int> ids, int accountId)
    {
        var listSchedule = _unitOfWork.WorkScheduleRepository.GetByIds(ids);
        if (listSchedule.Any())
        {
            return listSchedule.All(x => x.CreatedBy == accountId);
        }

        return false;
    }

    public WorkScheduleModel CheckIsAllDay(WorkSchedule schedule)
    {
        var timezone = _httpContext.User.GetTimezone();
        var startDate = schedule.StartDate.ConvertDateTimeUTCToSystemTimeZone(timezone);
        var endDate = schedule.EndDate.ConvertDateTimeUTCToSystemTimeZone(timezone);
        var scheduleModel = _mapper.Map<WorkScheduleModel>(schedule);
        // if (startDate.Hour == 0 && startDate.Minute == 0 && startDate.Second == 0 && endDate.Hour == 23 && endDate.Minute == 59 && endDate.Second == 59)
        // {
        //     scheduleModel.IsAllDay = true;
        // }
        if (startDate.TimeOfDay == TimeSpan.Zero && endDate.TimeOfDay == TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) + TimeSpan.FromSeconds(59))
        {
            scheduleModel.IsAllDay = true;
        }

        return scheduleModel;
    }
}