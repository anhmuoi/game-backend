using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.DataModel.MeetingRoom;
using Newtonsoft.Json;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class MeetingRoomMapping : Profile
{
    public MeetingRoomMapping()
    {


        CreateMap<MeetingRoom, MeetingRoomModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PasswordRoom, opt => opt.MapFrom(src => src.PasswordRoom))
            .ForMember(dest => dest.Default, opt => opt.MapFrom(src => src.Default))

            .ForMember(dest => dest.IsRunning, opt => opt.MapFrom(src => src.IsRunning))
            .ForMember(dest => dest.CurrentMeetingLogId, opt => opt.MapFrom(src => src.CurrentMeetingLogId))
            .ForMember(dest => dest.TotalPeople, opt => opt.MapFrom(src => src.TotalPeople))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.UserListId, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<int>>(src.UserListId)))
            .ForMember(dest => dest.CurrentPeople, opt => opt.MapFrom(src => src.CurrentPeople));

         
        CreateMap<MeetingRoomModel, MeetingRoom>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PasswordRoom, opt => opt.MapFrom(src => src.PasswordRoom))
            .ForMember(dest => dest.Default, opt => opt.MapFrom(src => src.Default))

            .ForMember(dest => dest.IsRunning, opt => opt.MapFrom(src => src.IsRunning))
            .ForMember(dest => dest.CurrentMeetingLogId, opt => opt.MapFrom(src => src.CurrentMeetingLogId))
            .ForMember(dest => dest.UserListId, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.UserListId)))
            .ForMember(dest => dest.TotalPeople, opt => opt.MapFrom(src => src.TotalPeople))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.CurrentPeople, opt => opt.MapFrom(src => src.CurrentPeople));
        CreateMap<MeetingRoom, MeetingRoomResponseModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PasswordRoom, opt => opt.MapFrom(src => src.PasswordRoom))
            .ForMember(dest => dest.Default, opt => opt.MapFrom(src => src.Default))

            .ForMember(dest => dest.IsRunning, opt => opt.MapFrom(src => src.IsRunning))
            .ForMember(dest => dest.CurrentMeetingLogId, opt => opt.MapFrom(src => src.CurrentMeetingLogId))
            .ForMember(dest => dest.TotalPeople, opt => opt.MapFrom(src => src.TotalPeople))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.UserListId, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<int>>(src.UserListId)))
            .ForMember(dest => dest.CurrentPeople, opt => opt.MapFrom(src => src.CurrentPeople));
      
    }
}