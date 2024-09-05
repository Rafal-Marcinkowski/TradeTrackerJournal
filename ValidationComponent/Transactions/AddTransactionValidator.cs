using FluentValidation;
using SharedProject.Models;

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
            .GreaterThan(0).WithMessage("Wprowadź cenę wejścia");

        RuleFor(x => x.NumberOfShares)
            .GreaterThan(0).WithMessage("Wprowadź ilość akcji");

        RuleFor(x => x.PositionSize)
            .GreaterThan(0).WithMessage("Wprowadź wielkość pozycji");

        RuleFor(x => x.InformationLink)
            .Must(BeAValidUrl).WithMessage("Wklej poprawny link")
            .When(x => !string.IsNullOrEmpty(x.InformationLink));

        RuleFor(x => x.AvgSellPrice)
            .GreaterThan(0).WithMessage("Niepoprawna cena sprzedaży")
            .When(x => x.AvgSellPrice.HasValue);

        RuleFor(x => x)
            .Must(x => Math.Abs(x.EntryPrice * x.NumberOfShares - x.PositionSize) < 100m)
            .When(x => x.EntryPrice > 0 && x.NumberOfShares > 0)
            .WithMessage(x => $"Wielkość pozycji:  ({x.EntryPrice} * {x.NumberOfShares} = {x.NumberOfShares * x.EntryPrice})");

        RuleFor(x => x)
            .Must(x => x.NumberOfShares > 0)
            .When(x => x.PositionSize > 0 && x.EntryPrice > 0)
            .WithMessage(x =>
            {
                var numberOfShares = x.EntryPrice != 0 ? (int)(x.PositionSize / x.EntryPrice) : 0;
                return $"Ilość akcji: ({x.PositionSize} / {x.EntryPrice} = {numberOfShares})";
            });
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
