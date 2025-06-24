using DataAccess.Data;
using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using SharedProject.Views;
using StockNotepad.MVVM.Models;
using StockNotepad.Services;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class AddCompanyViewModel(CompanyData companyData, TTJApiClient notepadApiClient) : BindableBase
{
    private string _companyName;
    public string CompanyName
    {
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }

    private bool CanSave => !string.IsNullOrWhiteSpace(CompanyName);

    public ICommand SaveCommand => new RelayCommand(async () =>
    {
        var companies = await companyData.GetAllCompaniesAsync();

        if (companies.Any(c => c.CompanyName.Equals(CompanyName, StringComparison.OrdinalIgnoreCase)))
        {
            var dialog = new ErrorDialog
            {
                DialogText = "Spółka o tej nazwie już istnieje"
            };
            dialog.ShowDialog();
            return;
        }

        var confirmationDialog = new ConfirmationDialog
        {
            DialogText = $"Czy na pewno chcesz dodać: {CompanyName.ToUpper()}?"
        };
        confirmationDialog.ShowDialog();
        if (!confirmationDialog.Result) return;

        Company company = new()
        {
            CompanyName = CompanyName.ToUpper(),
            TransactionCount = 0,
            EventCount = 0,
            NoteCount = 0
        };
        await companyData.InsertCompanyAsync(company);

        var companyItemDto = new NotepadCompanyItemDto
        {
            CompanyName = CompanyName.ToUpper(),
        };

        await notepadApiClient.AddNotepadCompanyItemAsync(companyItemDto);
    }, () => CanSave);
    public ICommand CancelCommand => new RelayCommand(() => CompanyName = string.Empty);
}
