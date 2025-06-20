using SharedProject.Extensions;
using StockNotepad.MVVM.Models;
using StockNotepad.Services;
using System.Windows.Input;

namespace StockNotepad.MVVM.ViewModels;

public class StockNotepadOverviewViewModel(TTJApiClient apiClient) : BindableBase, INavigationAware
{
    private string _summaryBackup = string.Empty;
    private NotepadCompanyItemDto? selectedCompanyItem;
    public NotepadCompanyItemDto? SelectedCompanyItem
    {
        get => selectedCompanyItem;
        set => SetProperty(ref selectedCompanyItem, value);
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.TryGetValue("selectedCompany", out object? param)
            && param is string companyName)
        {
            var allCompanies = await apiClient.GetNotepadCompanyItemsAsync();
            SelectedCompanyItem = allCompanies
                .FirstOrDefault(x => x.CompanyName.Equals(companyName, StringComparison.OrdinalIgnoreCase));
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }

    private bool _isEditingSummary = false;
    public bool IsEditingSummary
    {
        get => _isEditingSummary;
        set => SetProperty(ref _isEditingSummary, value);
    }

    public ICommand ToggleEditSummaryCommand => new DelegateCommand(() =>
    {
        IsEditingSummary = !IsEditingSummary;
        if (IsEditingSummary)
        {
            _summaryBackup = SelectedCompanyItem.Summary.Content;
        }
        else
        {
            SelectedCompanyItem.Summary.Content = _summaryBackup;
        }
    });

    public ICommand SaveSummaryCommand => new DelegateCommand(async () =>
    {
        if (!_summaryBackup.Equals(SelectedCompanyItem.Summary.Content, StringComparison.Ordinal))
        {
            SelectedCompanyItem.Summary.UpdatedAt = DateTime.Now.TrimToSeconds();
            await apiClient.UpdateSummaryAsync(SelectedCompanyItem.Id, SelectedCompanyItem.Summary);
        }
        IsEditingSummary = false;
    });

    public ICommand CancelEditSummaryCommand => new DelegateCommand(() =>
    {
        SelectedCompanyItem.Summary.Content = _summaryBackup;
        IsEditingSummary = false;
    });

    public ICommand AddNewNoteCommand => new DelegateCommand(() =>
    {
        var newNote = new NoteDto
        {
            Title = "Nowa notatka",
            Content = "",
            CreatedAt = DateTime.Now.TrimToSeconds(),
            IsEditing = true
        };

        SelectedCompanyItem.Notes.Add(newNote);
    });

    public ICommand EditNoteCommand => new DelegateCommand<NoteDto>(note =>
    {
        note.IsEditing = !note.IsEditing;
        if (note.IsEditing)
        {
            note.TitleBackup = note.Title;
            note.ContentBackup = note.Content;
        }
        else
        {
            note.Title = note.TitleBackup;
            note.Content = note.ContentBackup;
        }
    });

    public ICommand SaveNoteCommand => new DelegateCommand<NoteDto>(async note =>
    {
        if (!note.TitleBackup.Equals(note.Title, StringComparison.Ordinal) ||
            !note.ContentBackup.Equals(note.Content, StringComparison.Ordinal))
        {
            if (note.Id > 0)
            {
                await apiClient.UpdateNoteAsync(note.Id, note);
            }
            else
            {
                await apiClient.AddNoteAsync(SelectedCompanyItem.Id, note);
            }
        }
        note.IsEditing = false;
    });

    public ICommand CancelEditNoteCommand => new DelegateCommand<NoteDto>(note =>
    {
        note.Title = note.TitleBackup;
        note.Content = note.ContentBackup;
        note.IsEditing = false;
    });

    public ICommand DeleteNoteCommand => new DelegateCommand<NoteDto>(async note =>
    {
        // czy na pewno diag
        SelectedCompanyItem?.Notes?.Remove(note);
        if (note.Id > 0)
        {
            await apiClient.DeleteNoteAsync(note.Id);
        }
    });
}
