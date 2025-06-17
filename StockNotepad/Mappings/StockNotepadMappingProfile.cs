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
            .ReverseMap()
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .AfterMap((dto, entity) =>
            {
                if (entity.Summary is not null)
                    entity.Summary.NotepadCompanyItem = entity;
            });

        CreateMap<CompanySummaryDto, CompanySummary>()
            .ForMember(dest => dest.NotepadCompanyItem, opt => opt.Ignore())
            .ForMember(dest => dest.NotepadCompanyItemId, opt => opt.Ignore());

        CreateMap<NoteDto, Note>()
            .ForMember(dest => dest.NotepadCompanyItem, opt => opt.Ignore())
            .ForMember(dest => dest.NotepadCompanyItemId, opt => opt.Ignore());

        CreateMap<NotepadCompanyItemDto, NotepadCompanyItemViewModel>().ReverseMap();
    }
}
