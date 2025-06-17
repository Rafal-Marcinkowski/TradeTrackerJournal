using FluentValidation;
using FluentValidation.Results;
using SharedProject.Extensions;
using SharedProject.ViewModels;
using System.Globalization;
using System.Text;

namespace ValidationComponent.Transactions;

public class AddTransactionValidator : AbstractValidator<TransactionEventViewModel>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.SelectedCompanyName)
            .NotEmpty().WithMessage("Wybierz spółkę");

        RuleFor(x => x.EntryDate.Replace(";", ":"))
            .NotEmpty().WithMessage("Data wejścia jest wymagana")
            .Must(date => DateTime.TryParse(date, out _)).WithMessage("Nieprawidłowy format daty")
            .Must(BeWithinTradingHours).WithMessage("Transakcja musi być między 9:00 a 17:05")
            .Must(date => DateTime.TryParse(date, out var d) && d <= DateTime.Now).WithMessage("Data nie może być z przyszłości");

        RuleFor(x => x.EntryPrice)
            .NotEmpty().WithMessage("Cena wejścia jest wymagana")
            .Must(p => p.TryParseCleanDecimal(out _)).WithMessage("Nieprawidłowy format ceny")
            .Must(p => p.TryParseCleanDecimal(out var v) && v > 0).WithMessage("Cena musi być większa od 0");

        RuleFor(x => x.NumberOfShares)
            .NotEmpty().WithMessage("Ilość akcji jest wymagana")
            .Must(n => int.TryParse(n, NumberStyles.None, CultureInfo.InvariantCulture, out _)).WithMessage("Nieprawidłowa ilość akcji (tylko liczby całkowite)")
            .Must(n => int.TryParse(n, NumberStyles.None, CultureInfo.InvariantCulture, out var v) && v > 0).WithMessage("Ilość akcji musi być większa od 0");

        RuleFor(x => x.PositionSize)
            .NotEmpty().WithMessage("Wielkość pozycji jest wymagana")
            .Must(p => p.TryParseCleanDecimal(out _)).WithMessage("Nieprawidłowy format wielkości pozycji")
            .Must(p => p.TryParseCleanDecimal(out var v) && v > 0).WithMessage("Wielkość pozycji musi być większa od 0");

        RuleFor(x => x.AvgSellPrice)
            .Must(p => string.IsNullOrWhiteSpace(p) || p.TryParseCleanDecimal(out _)).WithMessage("Nieprawidłowy format ceny sprzedaży")
            .Must(p => string.IsNullOrWhiteSpace(p) || (p.TryParseCleanDecimal(out var v) && v > 0)).WithMessage("Cena sprzedaży musi być większa od 0");

        RuleFor(x => x)
            .Must(x =>
            {
                return x.EntryPrice.TryParseCleanDecimal(out var entryPrice)
                    && int.TryParse(x.NumberOfShares, NumberStyles.None, CultureInfo.InvariantCulture, out var numberOfShares)
                    && x.PositionSize.TryParseCleanDecimal(out var positionSize)
                    && Math.Abs((entryPrice * numberOfShares) - positionSize) < 100m;
            })
          .WithMessage(x =>
          {
              if (!int.TryParse(x.NumberOfShares, NumberStyles.None, CultureInfo.InvariantCulture, out var numberOfShares))
                  return "";
              if (!x.EntryPrice.TryParseCleanDecimal(out var entryPrice))
                  return "";
              if (!x.PositionSize.TryParseCleanDecimal(out var positionSize))
                  return "";

              var calculated = entryPrice * numberOfShares;
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

    private bool BeWithinTradingHours(string date)
    {
        if (DateTime.TryParse(date, out var parsedDate))
        {
            var time = parsedDate.TimeOfDay;
            return time >= TimeSpan.FromHours(9) && time <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(5));
        }
        return false;
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
               uri.Host.Contains('.');
    }

    public static string BuildGroupedValidationMessage(IEnumerable<ValidationFailure> failures)
    {
        var grouped = failures.GroupBy(f =>
        {
            var name = f.PropertyName;

            if (name == "SelectedCompanyName") return "🏢 Spółka";
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
