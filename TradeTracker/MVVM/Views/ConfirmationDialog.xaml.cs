using System.Windows;

namespace TradeTracker.MVVM.Views;

public partial class ConfirmationDialog
{
    public ConfirmationDialog()
    {
        InitializeComponent();
        this.Left = App.Current.MainWindow.Left + 625;
        this.Top = App.Current.MainWindow.Top + 400;
    }

    public bool Result { get; private set; }

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
