using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.FriendUser;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class FriendUserMapping : Profile
{
    /// <summary>
    /// FriendUser mapping
    /// </summary>
    public FriendUserMapping()
    {
        CreateMap<FriendUserModel, FriendUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId1, opt => opt.MapFrom(src => src.UserId1))
            .ForMember(dest => dest.UserId2, opt => opt.MapFrom(src => src.UserId2));

        CreateMap<FriendUser, FriendUserModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId1, opt => opt.MapFrom(src => src.UserId1))
            .ForMember(dest => dest.UserId2, opt => opt.MapFrom(src => src.UserId2))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));


        CreateMap<FriendUser, FriendUserListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId1, opt => opt.MapFrom(src => src.UserId1))
            .ForMember(dest => dest.UserId2, opt => opt.MapFrom(src => src.UserId2))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));

        CreateMap<FriendUserEditModel, FriendUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId1, opt => opt.MapFrom(src => src.UserId1))
            .ForMember(dest => dest.UserId2, opt => opt.MapFrom(src => src.UserId2));
        CreateMap<FriendUserAddModel, FriendUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId1, opt => opt.MapFrom(src => src.UserId1))
            .ForMember(dest => dest.UserId2, opt => opt.MapFrom(src => src.UserId2))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
    }
    
}