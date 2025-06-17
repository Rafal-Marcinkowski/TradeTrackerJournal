using DataAccess.Data;
using Infrastructure.Services;
using SharedProject.Models;
using SharedProject.ViewModels;
using StockNotepad.MVVM.Views;
using System.Windows.Input;

namespace StockNotepad.MVVM.ViewModels;

public class StockNotepadSelectCompanyViewModel : BaseListViewModel<Company>
{
    private readonly ICompanyData companyData;
    private readonly ViewManager viewManager;

    public StockNotepadSelectCompanyViewModel(ICompanyData companyData, ViewManager viewManager)
    {
        this.companyData = companyData;
        this.viewManager = viewManager;
        _ = LoadCompaniesAsync();
    }

    protected override void OnCollectionFiltered()
        => ItemsSource = [.. ItemsSource.OrderBy(q => q.CompanyName)];

    private async Task LoadCompaniesAsync()
    {
        ItemsSource = [.. await companyData.GetAllCompaniesAsync()];
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
