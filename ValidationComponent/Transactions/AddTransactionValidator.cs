using FluentValidation;
using SharedProject.Extensions;
using SharedProject.ViewModels;
using System.Globalization;

namespace ValidationComponent.Transactions;

public class AddTransactionValidator : AbstractValidator<TransactionEventViewModel>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.SelectedCompanyName)
            .NotEmpty().WithMessage("Wybierz spółkę");

        RuleFor(x => x.EntryDate)
            .NotEmpty().WithMessage("Data wejścia jest wymagana")
            .Must(BeValidDateTimeFormat).WithMessage("Nieprawidłowy format daty")
            .Must(BeWithinTradingHours).WithMessage("Transakcja musi być między 9:00 a 17:05")
            .Must(BeNotFutureDate).WithMessage("Data nie może być z przyszłości");

        RuleFor(x => x.EntryPrice)
            .NotEmpty().WithMessage("Cena wejścia jest wymagana")
            .Must(BeValidDecimal).WithMessage("Nieprawidłowy format ceny")
            .Must((_, price) =>
            {
                try
                {
                    return decimal.Parse(price.CleanNumberInput(), CultureInfo.InvariantCulture) > 0;
                }
                catch
                {
                    return false;
                }
            }).WithMessage("Cena musi być większa od 0");

        RuleFor(x => x.NumberOfShares)
            .NotEmpty().WithMessage("Ilość akcji jest wymagana")
            .Must(BeValidInteger).WithMessage("Nieprawidłowa ilość akcji (tylko liczby całkowite)")
            .Must((_, shares) =>
            {
                try
                {
                    var cleaned = shares.CleanNumberInput();
                    if (string.IsNullOrEmpty(cleaned)) return false;
                    return int.Parse(cleaned, CultureInfo.InvariantCulture) > 0;
                }
                catch
                {
                    return false;
                }
            }).WithMessage("Ilość akcji musi być większa od 0");

        RuleFor(x => x.PositionSize)
     .NotEmpty().WithMessage("Wielkość pozycji jest wymagana")
     .Must(BeValidDecimal).WithMessage("Nieprawidłowy format wielkości pozycji")
     .Must((_, size) => SafeDecimalParse(size) > 0)
     .WithMessage("Wielkość pozycji musi być większa od 0")
     .Must((_, size) => SafeDecimalParse(size) <= 100000000)
     .WithMessage("Wielkość pozycji nie może przekraczać 100 000 000 \n");

        RuleFor(x => x.AvgSellPrice)
            .Must(BeValidDecimalOrEmpty).WithMessage("Nieprawidłowy format ceny sprzedaży")
            .When(x => !string.IsNullOrWhiteSpace(x.AvgSellPrice))
            .Must((_, price) => SafeDecimalParse(price) > 0)
            .WithMessage("Cena sprzedaży musi być większa od 0")
            .When(x => !string.IsNullOrWhiteSpace(x.AvgSellPrice));

        RuleFor(x => x)
            .Must(x =>
            {
                try
                {
                    var entryPrice = SafeDecimalParse(x.EntryPrice);
                    var numberOfShares = SafeIntegerParse(x.NumberOfShares);
                    var positionSize = SafeDecimalParse(x.PositionSize);
                    return Math.Abs((entryPrice * numberOfShares) - positionSize) < 100m;
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage(x =>
            {
                try
                {
                    var entryPrice = SafeDecimalParse(x.EntryPrice);
                    var numberOfShares = SafeIntegerParse(x.NumberOfShares);
                    return $"Niespójność danych: Cena ({x.EntryPrice}) × Ilość ({x.NumberOfShares}) = {entryPrice * numberOfShares}, a pozycja: {x.PositionSize}";
                }
                catch
                {
                    return "Niespójność danych: nieprawidłowe wartości liczbowe";
                }
            });

        RuleFor(x => x.InitialDescription)
            .MaximumLength(2000).WithMessage("Opis początkowy nie może przekraczać 2000 znaków");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Opis nie może przekraczać 2000 znaków");
    }

    private decimal SafeDecimalParse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0m;

        try
        {
            var cleaned = input.CleanNumberInput();
            return decimal.Parse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        catch
        {
            return 0m;
        }
    }

    private int SafeIntegerParse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;

        try
        {
            var cleaned = input.CleanNumberInput(isInteger: true);
            return int.Parse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        catch
        {
            return 0;
        }
    }

    private bool BeValidDateTimeFormat(string date)
    {
        return DateTime.TryParse(date, out _);
    }

    private bool BeWithinTradingHours(string date)
    {
        if (DateTime.TryParse(date, out DateTime parsedDate))
        {
            var time = parsedDate.TimeOfDay;
            return time >= TimeSpan.FromHours(9) && time <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(5));
        }

        return false;
    }

    private bool BeNotFutureDate(string date)
    {
        if (DateTime.TryParse(date, out DateTime parsedDate))
        {
            return parsedDate <= DateTime.Now;
        }

        return false;
    }

    private bool BeValidDecimal(string number)
    {
        return decimal.TryParse(number.CleanNumberInput(), NumberStyles.Any, CultureInfo.InvariantCulture, out _);
    }

    private bool BeValidInteger(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return false;

        try
        {
            var cleaned = number.CleanNumberInput();
            return int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        catch
        {
            return false;
        }
    }

    private bool BeValidDecimalOrEmpty(string number)
    {
        return string.IsNullOrWhiteSpace(number) || BeValidDecimal(number);
    }
}
