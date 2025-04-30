using DataAccess.Data;
using Infrastructure.Events;
using Infrastructure.Interfaces;
using Serilog;
using SharedProject.Models;
using SharedProject.ViewModels;
using SharedProject.Views;
using System.Globalization;
using System.Windows;
using ValidationComponent.Transactions;

namespace Infrastructure.Services;

public class TransactionManager(ITransactionData transactionData, ICompanyData companyData,
        IEventAggregator eventAggregator) : BindableBase, ITransactionManager
{
    public async Task AddTransaction(Transaction Transaction)
    {
        Transaction.CompanyID = await companyData.GetCompanyID(Transaction.CompanyName);

        if (Transaction.AvgSellPrice != null)
        {
            Transaction.CloseDate = DateTime.Now;
            Transaction.IsClosed = true;
        }

        await transactionData.InsertTransactionAsync(Transaction);

        var company = await companyData.GetCompanyAsync(Transaction.CompanyID);
        company.TransactionCount++;
        await companyData.UpdateCompanyAsync(company.ID, Transaction.CompanyName,
            company.TransactionCount, company.EventCount);

        Transaction.ID = await transactionData.GetID(Transaction);
        eventAggregator.GetEvent<TransactionAddedEvent>().Publish(Transaction);
    }

    public async Task<Transaction> GetTransaction(int transactionId)
    {
        return await transactionData.GetTransactionAsync(transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactions() => await transactionData.GetAllTransactionsAsync();

    public async Task CloseTransaction(Transaction transaction)
    {
        var validator = new CloseTransactionValidation();
        var results = validator.Validate(transaction);

        if (!results.IsValid)
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));
            MessageBox.Show(validationErrors, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (transaction.AvgSellPrice > 0)
        {
            var dialog = new ConfirmationDialog()
            {
                DialogText = "Czy na pewno chcesz zamknąć transakcję?\n" +
                $"{transaction.CompanyName} {transaction.EntryDate}\nŚrednia cena sprzedaży: {transaction.AvgSellPrice}"
            };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                var dialog2 = new FinalizeTransactionDialog();
                dialog2.ShowDialog();

                if (dialog2.IsConfirmed)
                {
                    var closingComment = dialog2.ClosingComment;
                    transaction.ClosingDescription = closingComment;
                }

                transaction.IsClosed = true;
                transaction.CloseDate = DateTime.Now.Date
                             .AddHours(DateTime.Now.Hour)
                             .AddMinutes(DateTime.Now.Minute);
                await transactionData.UpdateTransactionAsync(transaction);
            }
        }
    }

    public async Task<IEnumerable<Transaction>> GetOpenTransactions()
    {
        var Transactions = await transactionData.GetAllTransactionsAsync();
        return Transactions.Where(t => !t.IsClosed).OrderByDescending(t => t.EntryDate);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsForCompany(int companyId)
    {
        return await transactionData.GetAllTransactionsForCompany(companyId);
    }

    public async Task<IEnumerable<Transaction>> GetLastXTransactions(int count)
    {
        var Transactions = await transactionData.GetAllTransactionsAsync();
        return Transactions.OrderByDescending(t => t.EntryDate).Take(count);
    }

    public async Task UpdateTransaction(Transaction Transaction)
    {
        await transactionData.UpdateTransactionAsync(Transaction);
        eventAggregator.GetEvent<TransactionUpdatedEvent>().Publish(Transaction);
    }

    public async Task<bool> TryAddTransaction(TransactionEventViewModel transactionVM)
    {
        Transaction transaction = await FillNewTransactionProperties(transactionVM);

        if (!await ValidateNewTransactionProperties(transaction)) return false;

        try
        {
            if (transaction.AvgSellPrice != null)
            {
                await SetClosingProperties(transaction);
            }

            transaction.CompanyID = await companyData.GetCompanyID(transaction.CompanyName);

            if (!await CheckTransactionValidity(transaction)) return false;

            if (await ConfirmTransaction(transaction)) return true;
        }

        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
        return false;
    }

    private async Task<bool> ConfirmTransaction(Transaction transaction)
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = $"Czy dodać transakcję? \n" +
                  $"{transaction.CompanyName}\n" +
                  $"{transaction.EntryDate}\n" +
                  $"Cena kupna: {transaction.EntryPrice}\n" +
                  $"Ilość akcji: {transaction.NumberOfShares}\n" +
                  $"Wielkość pozycji: {transaction.PositionSize}\n" +
                  $"{(transaction.AvgSellPrice != null ? $"Cena sprzedaży: {transaction.AvgSellPrice.ToString().Replace(',', '.')}" : string.Empty)}"
        };

        dialog.ShowDialog();

        if (dialog.Result)
        {
            await transactionData.InsertTransactionAsync(transaction);
            var company = await companyData.GetCompanyAsync(transaction.CompanyID);
            company.TransactionCount++;
            await companyData.UpdateCompanyAsync(company.ID, transaction.CompanyName, company.TransactionCount, company.EventCount);
            //var updatedCompany = ItemsSource.FirstOrDefault(c => c.ID == company.ID);

            //if (updatedCompany != null)
            //{
            //    updatedCompany.TransactionCount = company.TransactionCount;    odswiezanie UI po dodaniu?
            //}

            await transactionData.UpdateTransactionAsync(transaction);
            transaction.ID = await transactionData.GetID(transaction);
            eventAggregator.GetEvent<TransactionAddedEvent>().Publish(transaction);
            return true;
        }

        return false;
    }

    private async Task<bool> ValidateNewTransactionProperties(Transaction transaction)
    {
        var validator = new AddTransactionValidator();
        var results = validator.Validate(transaction);

        if (!results.IsValid)
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));

            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };

            dialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<bool> CheckTransactionValidity(Transaction transaction)
    {
        if (await new CheckTransaction(transactionData).IsExisting(transaction))
        {
            var errorDialog = new ErrorDialog()
            {
                DialogText = $"Transakcja już istnieje w bazie danych!"
            };

            errorDialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<Transaction> SetClosingProperties(Transaction transaction)
    {
        transaction.CloseDate = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
        transaction.IsClosed = true;

        TimeSpan timeSpan = (TimeSpan)(transaction.CloseDate - transaction.EntryDate);
        if (timeSpan.TotalDays > 7)
        {
            transaction.ClosingDescription = "Transakcja z archiwum.";
        }

        else
        {
            var dialog = new FinalizeTransactionDialog();
            dialog.ShowDialog();

            if (dialog.IsConfirmed)
            {
                var closingComment = dialog.ClosingComment.Trim();
                transaction.ClosingDescription = closingComment;
            }
        }

        return transaction;
    }

    public async Task<IEnumerable<Transaction>> LoadAndSetOpenTransactions()
    {
        var allTransactions = (await transactionData.GetAllTransactionsAsync()).Where(q => !q.IsClosed);

        foreach (var transaction in allTransactions)
        {
            transaction.Duration = await DateTimeManager.SetDuration(transaction.EntryDate);
        }

        return allTransactions;
    }

    private async Task<Transaction> FillNewTransactionProperties(TransactionEventViewModel transactionVM)
    {
        Transaction transaction = new()
        {
            CompanyName = transactionVM.SelectedCompanyName,
            EntryDate = DateTimeManager.ParseEntryDate(transactionVM.EntryDate),
            EntryPrice = decimal.TryParse(
            transactionVM.EntryPrice.Replace(" ", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                     .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator),
            NumberStyles.Any,
            CultureInfo.CurrentCulture,
            out var entryPrice)
            ? entryPrice
            : 0,
            NumberOfShares = int.TryParse(transactionVM.NumberOfShares.Where(x => !char.IsWhiteSpace(x)).ToArray(), out var numberOfShares) ? numberOfShares : 0,
            PositionSize = decimal.TryParse(transactionVM.PositionSize.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out decimal positionSize) ? positionSize : 0,
            InformationLink = transactionVM.InformationLink.Trim(),
            AvgSellPrice = decimal.TryParse(transactionVM.AvgSellPrice.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out var avgSellPrice) ? avgSellPrice : (decimal?)null,
            InitialDescription = transactionVM.InitialDescription.Trim(),
            Description = transactionVM.Description.Trim()
        };

        return transaction;
    }
}
