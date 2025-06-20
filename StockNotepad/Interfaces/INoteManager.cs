using StockNotepad.MVVM.Models;

namespace StockNotepad.Interfaces;
public interface INoteManager
{
    void EditNote(NoteDto note);
    Task SaveNote(NoteDto note, int companyItemId, string companyName);
    Task<bool> TryDeleteNote(NoteDto noteDto, string companyName);
}