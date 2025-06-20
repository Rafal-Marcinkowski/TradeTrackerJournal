using FluentValidation;
using FluentValidation.Results;
using SharedProject.Models;
using System.Text;

namespace ValidationComponent.Transactions;

public class AddTransactionValidator : AbstractValidator<Transaction>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Wybierz spółkę");

        RuleFor(x => x.EntryDate)
            .NotEmpty().WithMessage("Data wejścia jest wymagana")
            .Must(BeWithinTradingHours).WithMessage("Transakcja musi być między 9:00 a 17:05")
            .Must(d => d <= DateTime.Now).WithMessage("Data nie może być z przyszłości");

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0).WithMessage("Cena musi być większa od 0");

        RuleFor(x => x.NumberOfShares)
            .GreaterThan(0).WithMessage("Ilość akcji musi być większa od 0");

        RuleFor(x => x.AvgSellPrice)
            .GreaterThan(0).When(x => x.AvgSellPrice.HasValue)
            .WithMessage("Cena sprzedaży musi być większa od 0");

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.EntryPrice <= 0 || x.NumberOfShares <= 0 || x.PositionSize <= 0)
                    return false;

                var calculated = x.EntryPrice * x.NumberOfShares;
                return Math.Abs(calculated - x.PositionSize) < 100m;
            })
            .WithMessage(x =>
            {
                var calculated = x.EntryPrice * x.NumberOfShares;

                if (x.PositionSize <= 0)
                    return $"Wprowadź wielkość pozycji — powinna wynosić około {calculated}";

                return $"Niespójność danych: Cena ({x.EntryPrice}) × Ilość ({x.NumberOfShares}) = {calculated}, a pozycja: {x.PositionSize}";
            });

        RuleFor(x => x.InformationLink)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.InformationLink))
            .WithMessage("Niepoprawny format URL");

        RuleFor(x => x.InitialDescription)
            .MaximumLength(250)
            .When(x => !string.IsNullOrEmpty(x.InitialDescription))
            .WithMessage("Tytuł nie może przekraczać 250 znaków");

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Opis nie może przekraczać 4000 znaków");
    }

    private bool BeWithinTradingHours(DateTime date)
    {
        var time = date.TimeOfDay;
        return time >= TimeSpan.FromHours(9) && time <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(5));
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && uri.Host.Contains('.');
    }

    public static string BuildGroupedValidationMessage(IEnumerable<ValidationFailure> failures)
    {
        var grouped = failures.GroupBy(f =>
        {
            var name = f.PropertyName;

            if (name == "CompanyName") return "🏢 Spółka";
            if (name == "EntryDate") return "📅 Data";
            if (name == "EntryPrice" || name == "PositionSize" || name == "AvgSellPrice") return "💰 Cena/Wielkość pozycji";
            if (name == "NumberOfShares") return "📦 Wolumen";
            if (name == "InformationLink") return "🔗 Link";
            if (name == "InitialDescription") return "🏷️ Tytuł";
            if (name == "Description") return "📝 Opis";
            if (string.IsNullOrWhiteSpace(name)) return "⚠️ Spójność danych";

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
