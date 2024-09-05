using FluentValidation;
using SharedProject.Models;

namespace ValidationComponent.Transactions;

public class CloseTransactionValidation : AbstractValidator<Transaction>
{
    public CloseTransactionValidation()
    {
        RuleFor(x => x.AvgSellPrice)
             .GreaterThan(0).NotNull().WithMessage("Ustaw poprawną cenę sprzedaży");
    }
}
