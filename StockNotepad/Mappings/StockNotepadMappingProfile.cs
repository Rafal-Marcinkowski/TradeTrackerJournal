using AutoMapper;
using EFCore.Models;
using StockNotepad.MVVM.Models;
using StockNotepad.MVVM.ViewModels;

namespace StockNotepad.Mappings;

public class StockNotepadMappingProfile : Profile
{
    public StockNotepadMappingProfile()
    {
        CreateMap<NotepadCompanyItem, NotepadCompanyItemDto>()
        .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
        .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
        .ReverseMap()
        .ForMember(dest => dest.Summary, opt => opt.Ignore())
        .ForMember(dest => dest.Notes, opt => opt.Ignore());

        CreateMap<CompanySummary, CompanySummaryDto>().ReverseMap();
        CreateMap<Note, NoteDto>().ReverseMap();

        CreateMap<NotepadCompanyItemDto, NotepadCompanyItemViewModel>().ReverseMap();
    }
}
