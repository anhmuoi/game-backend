using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.FolderLog;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class FolderLogMapping : Profile
{
    public FolderLogMapping()
    {
        CreateMap<FolderLog, FolderLogModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId));
        CreateMap<FolderLog, FolderLogListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId ?? 0));
        CreateMap<FolderLogModel, FolderLog>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId));
        CreateMap<FolderLog, FolderLogSearch>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}