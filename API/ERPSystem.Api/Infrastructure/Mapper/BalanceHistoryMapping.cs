using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.BalanceHistory;

namespace ERPSystem.Api.Infrastructure.Mapper;

public class BalanceHistoryMapping : Profile
{
    /// <summary>
    /// BalanceHistory mapping
    /// </summary>
    public BalanceHistoryMapping()
    {
        CreateMap<BalanceHistoryModel, BalanceHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletAddress, opt => opt.MapFrom(src => src.WalletAddress))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<BalanceHistory, BalanceHistoryModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletAddress, opt => opt.MapFrom(src => src.WalletAddress))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));


        CreateMap<BalanceHistory, BalanceHistoryListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletAddress, opt => opt.MapFrom(src => src.WalletAddress))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn.ConvertDefaultDateTimeToString(Constants.Settings.DateTimeFormatDefault)));

        CreateMap<BalanceHistoryEditModel, BalanceHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletAddress, opt => opt.MapFrom(src => src.WalletAddress))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<BalanceHistoryAddModel, BalanceHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletAddress, opt => opt.MapFrom(src => src.WalletAddress))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ConvertDefaultStringToDateTime(Constants.Settings.DateTimeFormatDefault)));
    }
    
}