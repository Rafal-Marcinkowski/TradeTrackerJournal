using MahApps.Metro.Controls;
using System.Windows;

namespace SharedProject.Views;

public partial class ConfirmationDialog : MetroWindow
{
    public string DialogText
    {
        get { return (string)GetValue(DialogTextProperty); }
        set { SetValue(DialogTextProperty, value); }
    }

    public static readonly DependencyProperty DialogTextProperty =
        DependencyProperty.Register("DialogText", typeof(string), typeof(ConfirmationDialog), new PropertyMetadata(string.Empty));

    public bool Result { get; private set; }

    public ConfirmationDialog()
    {
        InitializeComponent();
        FontSize = 14;
        FontWeight = FontWeights.DemiBold;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        DataContext = this;
        this.Owner = Application.Current.MainWindow;
        this.MinHeight = 225;
        this.MinWidth = 350;
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}
