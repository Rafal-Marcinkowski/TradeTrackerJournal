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
            .ReverseMap();

        CreateMap<HotStockItem, HotStockItemDto>().ReverseMap();

        CreateMap<HotStockDayDto, HotStockDayViewModel>();
    }
}
