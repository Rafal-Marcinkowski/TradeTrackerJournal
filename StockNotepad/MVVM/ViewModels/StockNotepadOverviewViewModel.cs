using SharedProject.Extensions;
using StockNotepad.Interfaces;
using StockNotepad.MVVM.Models;
using StockNotepad.Services;
using System.Windows.Input;

namespace StockNotepad.MVVM.ViewModels;

public class StockNotepadOverviewViewModel(TTJApiClient apiClient, INoteManager noteManager) : BindableBase, INavigationAware
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

    public ICommand EditNoteCommand => new DelegateCommand<NoteDto>(note => noteManager.EditNote(note));

    public ICommand SaveNoteCommand => new DelegateCommand<NoteDto>(async note => await noteManager.SaveNote(note, SelectedCompanyItem.Id, SelectedCompanyItem.CompanyName));

    public ICommand CancelEditNoteCommand => new DelegateCommand<NoteDto>(note =>
    {
        if (note.Id == 0)
        {
            SelectedCompanyItem.Notes.Remove(note);
        }
        else
        {
            note.Title = note.TitleBackup;
            note.Content = note.ContentBackup;
            note.IsEditing = false;
        }
    });

    public ICommand DeleteNoteCommand => new DelegateCommand<NoteDto>(async note =>
    {

        if (note.Id == 0)
        {
            SelectedCompanyItem?.Notes?.Remove(note);
            return;
        }

        if (await noteManager.TryDeleteNote(note, SelectedCompanyItem.CompanyName))
            SelectedCompanyItem?.Notes?.Remove(note);
    });
}
