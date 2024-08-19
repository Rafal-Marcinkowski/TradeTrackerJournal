using MahApps.Metro.Controls;
using System.Windows;

namespace TradeTracker.MVVM.Views;

public partial class FinalizeTransactionDialog : MetroWindow
{
    public string ClosingComment { get; private set; }
    public bool IsConfirmed { get; private set; }

    public FinalizeTransactionDialog()
    {
        InitializeComponent();
        Top = App.Current.MainWindow.Top + 350;
        Left = App.Current.MainWindow.Left + 600;
        FontSize = 14;
        FontWeight = FontWeights.DemiBold;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        ClosingComment = ClosingCommentTextBox.Text;
        IsConfirmed = true;
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        IsConfirmed = false;
        this.Close();
    }
}
