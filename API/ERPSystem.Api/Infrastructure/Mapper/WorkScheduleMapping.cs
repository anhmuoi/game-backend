using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Dashboard;
using ERPSystem.DataModel.FolderLog;
using ERPSystem.DataModel.WorkSchedule;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class WorkScheduleMapping : Profile
{
    public WorkScheduleMapping()
    {
        CreateMap<WorkScheduleModel, WorkSchedule>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
         CreateMap<WorkSchedule, WorkScheduleModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<WorkSchedule, WorkScheduleListModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<WorkSchedule, DashboardDataModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));
        CreateMap<WorkSchedule, ChildFolderModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }
}