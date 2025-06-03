using AutoMapper;
using EFCore.Models;
using HotStockTracker.MVVM.ViewModels;
using SharedProject.Models;

namespace HotStockTracker.Mappings;

public class HotStockProfile : Profile
{
    public HotStockProfile()
    {
        CreateMap<HotStockDay, HotStockDayDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.HotStockItems))
            .ReverseMap();

        CreateMap<HotStockItem, HotStockItemDto>()
            .ReverseMap();

        CreateMap<HotStockDayDto, HotStockDayViewModel>();
    }
}
