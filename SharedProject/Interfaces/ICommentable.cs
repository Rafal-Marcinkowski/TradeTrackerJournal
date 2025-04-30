using SharedProject.Models;
using System.Collections.ObjectModel;

namespace SharedProject.Interfaces;

public interface ICommentable
{
    bool IsNewCommentBeingAdded { get; set; }
    string NewCommentText { get; set; }
    ObservableCollection<Comment> Comments { get; set; }
}
