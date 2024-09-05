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
        Top = Application.Current.MainWindow.Top + 400;
        Left = Application.Current.MainWindow.Left + 700;
        FontSize = 14;
        FontWeight = FontWeights.DemiBold;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        DataContext = this;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
