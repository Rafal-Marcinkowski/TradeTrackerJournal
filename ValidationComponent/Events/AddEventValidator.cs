using FluentValidation;
using SharedProject.Models;

namespace ValidationComponent.Events;

public class AddEventValidator : AbstractValidator<Event>
{
    public AddEventValidator()
    {
        RuleFor(x => x.CompanyName)
           .NotEmpty().WithMessage("Wybierz spółkę");

        RuleFor(x => x.EntryDate)
            .Must(BeAValidTime).WithMessage("Podaj poprawną datę wejścia")
            .When(x => !string.IsNullOrEmpty(x.EntryDate.ToString()));

        RuleFor(x => x.EntryPrice)
           .GreaterThan(0).WithMessage("Wprowadź początkowy kurs");

        RuleFor(x => x.InformationLink)
            .Must(BeAValidUrl).WithMessage("Wklej poprawny link")
            .When(x => !string.IsNullOrEmpty(x.InformationLink));
    }

    private bool BeAValidTime(DateTime entryDate)
    {
        var timeComponent = entryDate.TimeOfDay;
        return timeComponent >= TimeSpan.FromHours(9) && timeComponent <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(5));
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
