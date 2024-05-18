using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Driver;
using File = ERPSystem.DataAccess.Models.File;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class DriverMapping : Profile
{
    public DriverMapping()
    {
        CreateMap<FolderAddModel, Folder>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId));

        CreateMap<FolderEditModel, Folder>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<FileEditModel, File>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<UserFolder, UserShareModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.User.Department != null ? src.User.Department.Name : ""))
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType))
            .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => ((MediaPermission)(src.PermissionType)).GetDescription()));
        
        CreateMap<User, UserShareModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : ""))
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => -1));
        
        CreateMap<UserFile, UserShareModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.User.Department != null ? src.User.Department.Name : ""))
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType))
            .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => ((MediaPermission)(src.PermissionType)).GetDescription()));
    }
}