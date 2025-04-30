namespace SharedProject.Interfaces;

public interface IDetailable : ICommentable
{
    bool IsDetailsVisible { get; set; }
    bool IsCommentBeingEdited { get; set; }
}
