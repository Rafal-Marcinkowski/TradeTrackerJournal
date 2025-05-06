using MahApps.Metro.Controls;
using System.Windows;

namespace SharedProject.Views;

public partial class FinalizeTransactionDialog : MetroWindow
{
    public string ClosingComment { get; private set; }
    public bool IsConfirmed { get; private set; }

    public FinalizeTransactionDialog()
    {
        InitializeComponent();
        FontSize = 14;
        FontWeight = FontWeights.DemiBold;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        this.Owner = Application.Current.MainWindow;
        this.MinHeight = 225;
        this.MinWidth = 350;
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
