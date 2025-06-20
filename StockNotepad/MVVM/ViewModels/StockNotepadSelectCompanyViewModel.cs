using DataAccess.Data;
using Infrastructure.Services;
using SharedProject.Models;
using SharedProject.ViewModels;
using StockNotepad.MVVM.Views;
using StockNotepad.Services;
using System.Windows.Input;

namespace StockNotepad.MVVM.ViewModels;

public class StockNotepadSelectCompanyViewModel : BaseListViewModel<Company>
{
    private readonly ICompanyData companyData;
    private readonly ViewManager viewManager;
    private readonly TTJApiClient apiClient;

    public StockNotepadSelectCompanyViewModel(ICompanyData companyData, ViewManager viewManager, TTJApiClient apiClient)
    {
        this.companyData = companyData;
        this.viewManager = viewManager;
        this.apiClient = apiClient;
        _ = LoadCompaniesAsync();
    }

    protected override void OnCollectionFiltered()
        => ItemsSource = [.. ItemsSource.OrderByDescending(q => q.NoteCount)];

    private async Task LoadCompaniesAsync()
    {
        ItemsSource = [.. await companyData.GetAllCompaniesAsync()];
        OnCollectionFiltered();
    }

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany is not null)
        {
            var parameters = new NavigationParameters
        {
            {"selectedCompany", selectedCompany.CompanyName }
        };
            viewManager.NavigateTo(nameof(StockNotepadOverviewView), parameters);
        }
    });
}
