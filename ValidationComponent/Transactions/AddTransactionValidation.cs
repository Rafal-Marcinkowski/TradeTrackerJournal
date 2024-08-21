using FluentValidation;
using SharedModels.Models;

namespace ValidationComponent.Transactions;

public class AddTransactionValidator : AbstractValidator<Transaction>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Wybierz spółkę");

        RuleFor(x => x.EntryDate)
            .Must(BeAValidTime).WithMessage("Podaj poprawną datę wejścia")
            .When(x => !string.IsNullOrEmpty(x.EntryDate.ToString()));

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0).WithMessage("Cena wejścia musi być większa od 0");

        RuleFor(x => x.NumberOfShares)
            .GreaterThan(0).WithMessage("Ilość akcji musi być większa od 0");

        RuleFor(x => x.PositionSize)
            .GreaterThan(0).WithMessage("Wielkość pozycji musi być większa od 0");

        RuleFor(x => x.InformationLink)
            .Must(BeAValidUrl).WithMessage("Wklej poprawny link")
            .When(x => !string.IsNullOrEmpty(x.InformationLink));

        RuleFor(x => x.AvgSellPrice)
            .GreaterThan(0).WithMessage("Średnia cena sprzedaży musi być większa od 0")
            .When(x => x.AvgSellPrice.HasValue);

        RuleFor(x => x)
            .Must(x => Math.Abs(x.EntryPrice * x.NumberOfShares - x.PositionSize) < 200m)
            .WithMessage(x => $"Cena wejścia * ilość akcji musi się zgadzać z wielkością pozycji ({x.EntryPrice} * {x.NumberOfShares} = {x.NumberOfShares * x.EntryPrice})");
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
