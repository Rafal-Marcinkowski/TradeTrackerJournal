using DataAccess.Data;
using Infrastructure.Services;
using SharedProject.Models;
using SharedProject.ViewModels;
using StockNotepad.MVVM.Models;
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
        => ItemsSource = [.. ItemsSource.OrderBy(q => q.CompanyName)];

    private async Task LoadCompaniesAsync()
    {
        ItemsSource = [.. await companyData.GetAllCompaniesAsync()];

        foreach (var item in ItemsSource)
        {
            await apiClient.AddNotepadCompanyItemAsync(new NotepadCompanyItemDto
            {
                CompanyName = item.CompanyName,
                Summary = new CompanySummaryDto
                {
                    Content = $"Podsumowanie dla: {item.CompanyName}",
                    UpdatedAt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                         DateTime.Now.Hour, DateTime.Now.Minute, 0),
                },
            }).ConfigureAwait(false);
            return;
        }
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
