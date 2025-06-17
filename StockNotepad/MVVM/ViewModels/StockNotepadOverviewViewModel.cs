using StockNotepad.MVVM.Models;
using StockNotepad.Services;
using System.Windows.Input;

namespace StockNotepad.MVVM.ViewModels;

public class StockNotepadOverviewViewModel : BindableBase, INavigationAware
{
    public StockNotepadOverviewViewModel(TTJApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    private string _summaryBackup;
    private readonly TTJApiClient apiClient;

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
            //SelectedCompanyItem = allCompanies
            //    .FirstOrDefault(x => x.CompanyName.Equals(companyName, StringComparison.OrdinalIgnoreCase));
            SelectedCompanyItem = new()
            {
                CompanyName = companyName
            };
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }

    private bool _isEditingSummary;
    public bool IsEditingSummary
    {
        get => _isEditingSummary;
        set => SetProperty(ref _isEditingSummary, value);
    }

    public ICommand ToggleEditSummaryCommand => new DelegateCommand(() =>
    {
        _summaryBackup = SelectedCompanyItem.Summary.Content;
        IsEditingSummary = true;
    });

    public ICommand SaveSummaryCommand => new DelegateCommand(() =>
    {
        SelectedCompanyItem.Summary.UpdatedAt = DateTime.Now;
        IsEditingSummary = false;
    });

    public ICommand CancelEditSummaryCommand => new DelegateCommand(() =>
    {
        SelectedCompanyItem.Summary.Content = _summaryBackup;
        IsEditingSummary = false;
    });

    //public ICommand AddNewNoteCommand => new DelegateCommand(() =>
    //{
    //    var newNote = new NoteDto
    //    {
    //        Title = "Nowa notatka",
    //        Content = "",
    //        CreatedAt = DateTime.Now,
    //        IsEditing = true
    //    };
    //    SelectedCompanyItem.Notes.Add(newNote);
    //});

    ////public ICommand EditNoteCommand => new DelegateCommand<NoteDto>(note =>
    ////{
    ////    note.IsEditing = true;
    ////    note.BackupTitle = note.Title;
    ////    note.BackupContent = note.Content;
    ////});

    ////public ICommand SaveNoteCommand => new DelegateCommand<NoteDto>(note =>
    ////{
    ////    note.IsEditing = false;
    ////});

    ////public ICommand CancelEditNoteCommand => new DelegateCommand<NoteDto>(note =>
    ////{
    ////    note.Title = note.BackupTitle;
    ////    note.Content = note.BackupContent;
    ////    note.IsEditing = false;
    ////});

    //public ICommand DeleteNoteCommand => new DelegateCommand<NoteDto>(note =>
    //{
    //    SelectedCompanyItem.Notes.Remove(note);
    //});



    //private void CancelEditSummary()
    //{
    //    SelectedCompanyItem.Summary.Content = _summaryBackup;
    //    IsEditingSummary = false;
    //}

    //// Włącz edycję: zachowaj backup treści
    //private void ToggleEditSummary()
    //{
    //    _summaryBackup = SelectedCompanyItem.Summary.Content;
    //    IsEditingSummary = true;
    //}

    //// Notatki:

    //private void AddNewNote()
    //{
    //    var newNote = new NoteDto
    //    {
    //        Title = "Nowa notatka",
    //        Content = "",
    //        CreatedAt = DateTime.Now,
    //        IsEditing = true
    //    };
    //    SelectedCompanyItem.Notes.Add(newNote);
    //}

    ////private void EditNote(NoteDto note)
    ////{
    ////    note.IsEditing = true;
    ////    note.BackupTitle = note.Title;
    ////    note.BackupContent = note.Content;
    ////}

    ////private void SaveNote(NoteDto note)
    ////{
    ////    note.IsEditing = false;
    ////}

    ////private void CancelEditNote(NoteDto note)
    ////{
    ////    note.Title = note.BackupTitle;
    ////    note.Content = note.BackupContent;
    ////    note.IsEditing = false;
    ////}

    //private void DeleteNote(NoteDto note)
    //{
    //    SelectedCompanyItem.Notes.Remove(note);
    //}
}
