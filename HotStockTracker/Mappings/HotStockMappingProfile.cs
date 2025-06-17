using AutoMapper;
using EFCore.Models;
using SharedProject.Models;

namespace HotStockTracker.Mappings;

public class HotStockMappingProfile : Profile
{
    public HotStockMappingProfile()
    {
        CreateMap<HotStockDay, HotStockDayDto>()
            .ForMember(dest => dest.HotStockItems, opt => opt.MapFrom(src => src.HotStockItems))
            .ReverseMap()
            .ForMember(dest => dest.HotStockItems, opt => opt.Ignore());

        CreateMap<HotStockItem, HotStockItemDto>()
            .ReverseMap()
            .ForMember(dest => dest.HotStockDay, opt => opt.Ignore())
            .ForMember(dest => dest.HotStockDayId, opt => opt.Ignore());

        CreateMap<HotStockDayDto, HotStockDayViewModel>();
    }
}
