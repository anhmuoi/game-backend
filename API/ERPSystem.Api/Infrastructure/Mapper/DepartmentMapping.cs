using AutoMapper;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Department;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class DepartmentMapping : Profile
{
    public DepartmentMapping()
    {
        CreateMap<DepartmentAddModel, Department>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DepartmentManagerId, opt => opt.MapFrom(src => src.DepartmentManagerId))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId));
        
        CreateMap<DepartmentEditModel, Department>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DepartmentManagerId, opt => opt.MapFrom(src => src.DepartmentManagerId))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId));
        
        CreateMap<Department, DepartmentListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DepartmentManagerId, opt => opt.MapFrom(src => src.DepartmentManagerId))
            .ForMember(dest => dest.DepartmentManager, opt => opt.MapFrom(src => src.DepartmentManager != null && src.DepartmentManager.User != null ? src.DepartmentManager.User.Name : null))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.DepartmentParent, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));
        
        CreateMap<Department, DepartmentModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DepartmentManagerId, opt => opt.MapFrom(src => src.DepartmentManagerId))
            .ForMember(dest => dest.DepartmentManager, opt => opt.MapFrom(src => src.DepartmentManager != null && src.DepartmentManager.User != null ? src.DepartmentManager.User.Name : null))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.DepartmentParent, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

        CreateMap<Department, DepartmentItemModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DepartmentManagerId, opt => opt.MapFrom(src => src.DepartmentManagerId))
            .ForMember(dest => dest.DepartmentManager, opt => opt.MapFrom(src => src.DepartmentManager != null && src.DepartmentManager.User != null ? src.DepartmentManager.User.Name : null))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.DepartmentParent, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

    }
}