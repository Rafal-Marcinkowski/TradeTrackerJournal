using Infrastructure.Services;
using SharedProject.Extensions;
using SharedProject.Views;
using StockNotepad.Interfaces;
using StockNotepad.MVVM.Models;

namespace StockNotepad.Services;

public class NoteManager(TTJApiClient apiClient, CompanyManager companyManager) : INoteManager
{
    public async Task<bool> TryDeleteNote(NoteDto noteDto, string companyName)
    {
        var dialog = new ConfirmationDialog
        {
            DialogText = "Czy na pewno chcesz usunąć tę notatkę?",
        };

        dialog.ShowDialog();

        if (!dialog.Result)
            return false;

        await apiClient.DeleteNoteAsync(noteDto.Id);
        await companyManager.DecrementNoteCount(companyName);
        return true;
    }

    public void EditNote(NoteDto note)
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
    }

    public async Task SaveNote(NoteDto note, int companyItemId, string companyName)
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
                note.CreatedAt = DateTime.Now.TrimToSeconds();
                int? id = await apiClient.AddNoteAsync(companyItemId, note);
                note.Id = (int)id;
                await companyManager.IncrementNoteCount(companyName);
            }
        }
        note.IsEditing = false;
    }
}
