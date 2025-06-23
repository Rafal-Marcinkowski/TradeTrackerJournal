using DataAccess.Data;
using SharedProject.Models;
using SharedProject.ViewModels;
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

    public string NewCompanyName
    {
        get => _newCompanyName;
        set => SetProperty(ref _newCompanyName, value);
    }

    public bool CanSave => SelectedCompany != null &&
                         !string.IsNullOrWhiteSpace(NewCompanyName) &&
                         NewCompanyName != SelectedCompany.CompanyName;

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((company) =>
    {
        SelectedCompany = company;
        NewCompanyName = company?.CompanyName;
    });

    public ICommand SaveCommand => new DelegateCommand(() => SelectedCompany.CompanyName = NewCompanyName, () => CanSave);

    public ICommand CancelCommand => new DelegateCommand(() =>
    {
        SelectedCompany = null;
        NewCompanyName = string.Empty;
    });

    public CompanyRenameViewModel(CompanyData companyData)
    {
        this.companyData = companyData;
        _ = LoadCompanies();
    }

    private async Task LoadCompanies() =>
        ItemsSource = [.. await companyData.GetAllCompaniesAsync()];
}
