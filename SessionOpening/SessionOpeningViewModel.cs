using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SessionOpening;

public class SessionOpeningViewModel : BindableBase
{
    private string newItemText = string.Empty;
    public string NewItemText
    {
        get => newItemText;
        set => SetProperty(ref newItemText, value);
    }

    private bool isNewItemBeingAdded = false;
    public bool IsNewItemBeingAdded
    {
        get => isNewItemBeingAdded;
        set
        {
            if (SetProperty(ref isNewItemBeingAdded, value))
                Vis = IsNewItemBeingAdded ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private Visibility visibility = Visibility.Collapsed;
    public Visibility Vis
    {
        get
        {
            if (IsNewItemBeingAdded)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        set => SetProperty(ref visibility, value);
    }

    public ObservableCollection<OpeningItem> OpeningItems { get; set; } = [];

    public ICommand AddNewItemCommand => new DelegateCommand(() =>
    {
        IsNewItemBeingAdded = !IsNewItemBeingAdded;
    });

    public ICommand ConfirmNewItemCommand => new DelegateCommand(() =>
    {
        OpeningItem item = new()
        {
            Text = NewItemText,
        };

        OpeningItems.Add(item);
        NewItemText = string.Empty;
        IsNewItemBeingAdded = !IsNewItemBeingAdded;
    });

    public ICommand ItemDoubleClickCommand => new DelegateCommand<OpeningItem>((item) =>
    {
        var url = item.Text;
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    });
}
