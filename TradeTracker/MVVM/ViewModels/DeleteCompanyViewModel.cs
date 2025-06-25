using DataAccess.Data;
using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using SharedProject.ViewModels;
using SharedProject.Views;
using StockNotepad.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class DeleteCompanyViewModel : BaseListViewModel<Company>
{
    private Company _selectedCompany;
    private readonly CompanyData _companyData;
    private readonly TTJApiClient _apiClient;

    public Company SelectedCompany
    {
        get => _selectedCompany;
        set => SetProperty(ref _selectedCompany, value);
    }

    public ICommand ConfirmCompanySelectionCommand => new RelayCommand<Company>(company => SelectedCompany = company);

    public ICommand DeleteCommand => new RelayCommand(async () =>
    {
        var confirmationDialog = new ConfirmationDialog
        {
            DialogText = $"Na pewno usunąć {SelectedCompany?.CompanyName}?"
        };

        confirmationDialog.ShowDialog();
        if (!confirmationDialog.Result) return;

        var id = await _companyData.GetCompanyID(SelectedCompany.CompanyName);

        await _companyData.DeleteCompanyAsync(id);

        var apiCompanyId = await _apiClient.GetCompanyIdByNameAsync(SelectedCompany.CompanyName);
        if (apiCompanyId is not null)
        {
            await _apiClient.DeleteNotepadCompanyItemAsync(apiCompanyId.Value);
        }

        Cancel();
        _ = LoadCompanies();
    }, () => SelectedCompany is not null);

    public ICommand CancelCommand => new RelayCommand(Cancel);

    public DeleteCompanyViewModel(CompanyData companyData, TTJApiClient apiClient)
    {
        _companyData = companyData;
        _apiClient = apiClient;

        _ = LoadCompanies();
    }

    private void Cancel()
    {
        SelectedCompany = null;
        SearchKeyword = string.Empty;
    }

    private async Task LoadCompanies() =>
        ItemsSource = new ObservableCollection<Company>(await _companyData.GetAllCompaniesAsync());

    protected override void OnCollectionFiltered() =>
        ItemsSource = new ObservableCollection<Company>(ItemsSource.OrderBy(c => c.CompanyName));
}
