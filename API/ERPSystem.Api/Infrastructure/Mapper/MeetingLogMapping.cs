using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.DataModel.MeetingLog;
using Newtonsoft.Json;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class MeetingLogMapping : Profile
{
    public MeetingLogMapping()
    {
        CreateMap<MeetingLog, DashboardDataModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<MeetingLog, ChildFolderModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        CreateMap<MeetingLog, MeetingLogModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UserList, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<UserData>>(src.UserList)))
            .ForMember(dest => dest.MeetingRoomId, opt => opt.MapFrom(src => src.MeetingRoomId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<MeetingLogModel, MeetingLog>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UserList, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.UserList)))
            .ForMember(dest => dest.MeetingRoomId, opt => opt.MapFrom(src => src.MeetingRoomId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<MeetingLog, MeetingLogResponseModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.MeetingRoomId, opt => opt.MapFrom(src => src.MeetingRoomId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UserList, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<UserData>>(src.UserList)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.Value.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<MeetingLog, FolderLogSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.GenerateCustomId(Constants.MeetingLogKey)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
    }
}