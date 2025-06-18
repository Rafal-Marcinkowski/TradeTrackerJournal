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
            .ForMember(dest => dest.Notes, opt => opt.Ignore());

        CreateMap<CompanySummaryDto, CompanySummary>()
            .ForMember(dest => dest.NotepadCompanyItem, opt => opt.Ignore())
            .ForMember(dest => dest.NotepadCompanyItemId, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<NoteDto, Note>()
            .ForMember(dest => dest.NotepadCompanyItem, opt => opt.Ignore())
            .ForMember(dest => dest.NotepadCompanyItemId, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<NotepadCompanyItemDto, NotepadCompanyItemViewModel>().ReverseMap();
    }
}
