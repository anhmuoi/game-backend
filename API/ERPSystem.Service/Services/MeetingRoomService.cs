using AutoMapper;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.MeetingRoom;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace ERPSystem.Service.Services;

public interface IMeetingRoomService
{
    List<MeetingRoomResponseModel> GetPaginated(string search, List<int> meetingRoomIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered);
    bool Add(MeetingRoomModel model);
    bool Update(MeetingRoomModel model);
    bool Delete(List<int> ids);
    MeetingRoomResponseModel? GetById(int id);
}
public class MeetingRoomService : IMeetingRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly HttpContext _httpContext;
    private readonly IMapper _mapper;
    
    public MeetingRoomService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<DailyReportService>();
        _mapper = mapper;
        _httpContext = contextAccessor.HttpContext;
    }

    public List<MeetingRoomResponseModel> GetPaginated(string search, List<int> meetingRoomIds, List<int> folderLogs, int pageNumber, int pageSize, string sortColumn,
        string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        List<MeetingRoomResponseModel> result = new List<MeetingRoomResponseModel>();
        var currentUser = _httpContext.User.GetAccountId();
        var data = _unitOfWork.MeetingRoomRepository.GetAll().AsQueryable().Where(m => m.IsDelete == false);
        
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(x => x.Name.ToLower().Contains(search.ToLower()));
        }
       

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        foreach (var item in data)
        {
            var meetingRoom = _mapper.Map<MeetingRoomResponseModel>(item);
            result.Add(meetingRoom);
        }
        return result;
    }

    public bool Add(MeetingRoomModel model)
    {
        try
        {
            var meetingRoom = _mapper.Map<MeetingRoom>(model);
            
            _unitOfWork.MeetingRoomRepository.Add(meetingRoom);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public bool Update(MeetingRoomModel model)
    {
        try
        {
            var meetingRoom = _unitOfWork.MeetingRoomRepository.GetById(model.Id);
            if (meetingRoom != null)
            {
                _mapper.Map(model, meetingRoom);
            
                _unitOfWork.MeetingRoomRepository.Update(meetingRoom);
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
            var meetingRooms = _unitOfWork.MeetingRoomRepository.GetByIds(ids);
            _unitOfWork.MeetingRoomRepository.DeleteRange(meetingRooms);
            _unitOfWork.Save();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return false;
        }
    }

    public MeetingRoomResponseModel? GetById(int id)
    {
        try
        {
            var meetingRoom = _unitOfWork.MeetingRoomRepository.GetById(id);
            if (meetingRoom != null)
            {
                var result = _mapper.Map<MeetingRoomResponseModel>(meetingRoom);
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

}