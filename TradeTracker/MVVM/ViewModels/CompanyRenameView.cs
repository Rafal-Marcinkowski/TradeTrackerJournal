using DataAccess.Data;
using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using SharedProject.ViewModels;
using SharedProject.Views;
using StockNotepad.Services;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class CompanyRenameViewModel : BaseListViewModel<Company>
{
    private Company _selectedCompany;
    public Company SelectedCompany
    {
        get => _selectedCompany;
        set => SetProperty(ref _selectedCompany, value);
    }

    private string _newCompanyName;
    private readonly CompanyData companyData;
    private readonly TTJApiClient notepadApiClient;

    public string NewCompanyName
    {
        get => _newCompanyName;
        set => SetProperty(ref _newCompanyName, value);
    }

    private bool CanSave =>
        SelectedCompany != null &&
        !string.IsNullOrWhiteSpace(NewCompanyName) &&
        NewCompanyName != SelectedCompany.CompanyName;

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((company) =>
    {
        SelectedCompany = company;
        NewCompanyName = company?.CompanyName;
    });

    public ICommand SaveCommand => new RelayCommand(async () =>
    {
        var confirmationDialog = new ConfirmationDialog
        {
            DialogText = $"Na pewno zmienić nazwę? {SelectedCompany?.CompanyName} => {NewCompanyName.ToUpper()}?"
        };

        confirmationDialog.ShowDialog();

        if (!confirmationDialog.Result) return;

        var id = await companyData.GetCompanyID(SelectedCompany.CompanyName);
        var company = await companyData.GetCompanyAsync(id);

        await companyData.UpdateCompanyAsync(company.ID, NewCompanyName.ToUpper(), company.TransactionCount, company.EventCount, company.NoteCount);

        var apiCompanyId = await notepadApiClient.GetCompanyIdByNameAsync(SelectedCompany.CompanyName);

        if (apiCompanyId is not null)
        {
            await notepadApiClient.UpdateCompanyNameAsync((int)apiCompanyId, NewCompanyName.ToUpper());
        }

        CancelCommand.Execute(null);
        await LoadCompanies();
    }, () => CanSave);

    public ICommand CancelCommand => new DelegateCommand(() =>
    {
        SelectedCompany = null;
        NewCompanyName = string.Empty;
        SearchKeyword = string.Empty;
    });

    public CompanyRenameViewModel(CompanyData companyData, TTJApiClient notepadApiClient)
    {
        this.companyData = companyData;
        this.notepadApiClient = notepadApiClient;
        _ = LoadCompanies();
    }

    private async Task LoadCompanies() =>
        ItemsSource = [.. await companyData.GetAllCompaniesAsync()];

    protected override void OnCollectionFiltered() =>
        ItemsSource = [.. ItemsSource.OrderBy(c => c.CompanyName)];
}