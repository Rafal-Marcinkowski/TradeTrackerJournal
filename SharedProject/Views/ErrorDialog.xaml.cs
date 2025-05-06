using MahApps.Metro.Controls;
using System.Windows;

namespace SharedProject.Views;

public partial class ErrorDialog : MetroWindow
{
    public string DialogText
    {
        get { return (string)GetValue(DialogTextProperty); }
        set { SetValue(DialogTextProperty, value); }
    }

    public static readonly DependencyProperty DialogTextProperty =
        DependencyProperty.Register("DialogText", typeof(string), typeof(ErrorDialog), new PropertyMetadata(string.Empty));

    public ErrorDialog()
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

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
