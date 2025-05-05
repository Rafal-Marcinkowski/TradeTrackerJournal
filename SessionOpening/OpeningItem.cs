namespace SessionOpening;

public class OpeningItem : BindableBase
{
    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    private bool isEditing = false;
    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }
}
