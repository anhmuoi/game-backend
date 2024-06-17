using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.ItemNft;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class ItemNftMapping : Profile
{
    /// <summary>
    /// ItemNft mapping
    /// </summary>
    public ItemNftMapping()
    {
        CreateMap<ItemNftModel, ItemNft>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<ItemNft, ItemNftModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));


        CreateMap<ItemNft, ItemNftListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));

        CreateMap<ItemNftEditModel, ItemNft>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<ItemNftAddModel, ItemNft>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
            .ForMember(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
    }
    
}