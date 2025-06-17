using FluentValidation;
using FluentValidation.Results;
using SharedProject.Models;
using System.Text;

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

        RuleFor(x => x.InitialDescription)
                .MaximumLength(250)
                .When(x => !string.IsNullOrEmpty(x.InitialDescription))
                .WithMessage("Tytuł nie może przekraczać 250 znaków");

        RuleFor(x => x.Description)
                .MaximumLength(4000)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Opis nie może przekraczać 4000 znaków");
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

    public static string BuildGroupedValidationMessage(IEnumerable<ValidationFailure> failures)
    {
        var grouped = failures.GroupBy(f =>
        {
            var name = f.PropertyName;

            if (name == "CompanyName") return "🏢 Spółka";
            if (name == "EntryDate") return "📅 Data wejścia";
            if (name == "EntryPrice") return "💰 Cena wejścia";
            if (name == "InformationLink") return "🔗 Link";
            return "❓ Inne";
        });

        var sb = new StringBuilder();
        foreach (var group in grouped)
        {
            sb.AppendLine(group.Key + ":");
            foreach (var error in group)
            {
                if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                    sb.AppendLine($"• {error.ErrorMessage}");
            }
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }
}
