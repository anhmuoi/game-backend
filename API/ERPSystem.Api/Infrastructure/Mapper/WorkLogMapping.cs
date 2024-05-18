using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.DataModel.WorkLog;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class WorkLogMapping : Profile
{
    public WorkLogMapping()
    {
        CreateMap<WorkLog, WorkLogModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
         CreateMap<WorkLogModel, WorkLog>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<WorkLog, WorkLogResponseModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<WorkLog, ChildFolderModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        CreateMap<WorkLog, FolderLogSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.GenerateCustomId(Constants.WorkLogKey)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
    }
}