using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.DailyReport;
using ERPSystem.DataModel.FolderLog;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class DailyReportMapping : Profile
{
    public DailyReportMapping()
    {
        CreateMap<DailyReport, DailyReportModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.ReporterId, opt => opt.MapFrom(src => src.ReporterId));
        CreateMap<DailyReportModel, DailyReport>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.ReporterId, opt => opt.MapFrom(src => src.ReporterId));
        CreateMap<DailyReport, DailyReportResponseModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.User.Department.Name))
            .ForMember(dest => dest.FolderLogId, opt => opt.MapFrom(src => src.FolderLogId))
            .ForMember(dest => dest.ReporterId, opt => opt.MapFrom(src => src.ReporterId));
        CreateMap<DailyReport, ChildFolderModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }
}