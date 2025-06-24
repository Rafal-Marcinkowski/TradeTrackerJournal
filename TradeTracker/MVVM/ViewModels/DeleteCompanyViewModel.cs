using DataAccess.Data;
using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using SharedProject.ViewModels;
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

    public ICommand ConfirmCompanySelectionCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    public DeleteCompanyViewModel(CompanyData companyData, TTJApiClient apiClient)
    {
        _companyData = companyData;
        _apiClient = apiClient;

        ConfirmCompanySelectionCommand = new RelayCommand<Company>(company =>
        {
            SelectedCompany = company;
        });

        DeleteCommand = new RelayCommand(async () => await DeleteCompany());
        CancelCommand = new RelayCommand(Cancel);

        LoadCompanies();
    }

    private async Task DeleteCompany()
    {
        //if (SelectedCompany == null) return;

        //try
        //{
        //    // Usunięcie z lokalnej bazy
        //    await _companyData.DeleteCompanyAsync(SelectedCompany.ID);

        //    // Usunięcie z API (jeśli potrzebne)
        //    var apiCompanyId = await _apiClient.GetCompanyIdByNameAsync(SelectedCompany.CompanyName);
        //    if (apiCompanyId.HasValue)
        //    {
        //        await _apiClient.DeleteCompanyAsync(apiCompanyId.Value);
        //    }

        //    SelectedCompany = null;
        //    await LoadCompanies();

        //    MessageBox.Show("Spółka została pomyślnie usunięta");
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Błąd podczas usuwania: {ex.Message}");
        //}
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
