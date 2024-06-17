using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.ItemNftUser;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class ItemNftUserMapping : Profile
{
    /// <summary>
    /// ItemNftUser mapping
    /// </summary>
    public ItemNftUserMapping()
    {
        CreateMap<ItemNftUserModel, ItemNftUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ItemNftId, opt => opt.MapFrom(src => src.ItemNftId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<ItemNftUser, ItemNftUserModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ItemNftId, opt => opt.MapFrom(src => src.ItemNftId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));


        CreateMap<ItemNftUser, ItemNftUserListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ItemNftId, opt => opt.MapFrom(src => src.ItemNftId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));

        CreateMap<ItemNftUserEditModel, ItemNftUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ItemNftId, opt => opt.MapFrom(src => src.ItemNftId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<ItemNftUserAddModel, ItemNftUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ItemNftId, opt => opt.MapFrom(src => src.ItemNftId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
    }
    
}